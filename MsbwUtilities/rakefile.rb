$: << File.expand_path(File.dirname(__FILE__))
require "msbuild"
require "tools"
require "config"
require "nunit"
require "albacore"

with('MsbwUtilities.sln') do |sln|
	BradyW::MSBuild.new :cleandnet do |clean|
		clean.targets = "clean"
		clean.solution = sln
	end

	BradyW::MSBuild.new :build do |build|
		build.solution = sln
	end
end

task :ci => [:clean, :build, :test]
task :clean => [:cleandnet, :cleanpackages]
task :test => [:codetest]
task :package => [:clean, :version, :build, :pack]

task :version => [:versionmsbswutil]
task :pack => [:packmsbwutil]
task :publish => [:publishmsbwutil]

with ('test') do |t|	
	BradyW::Nunit.new :codetest => :build do |test|
		test.files = FileList["#{t}/**/bin/Debug/*Test.dll"]
		# Since we have some expensive tests and our box is slow, let's do a timeout of 60 seconds
		test.timeout = 60000
	end	
end

task :cleanpackages do
	rm_rf FileList['**/*/*.nupkg']
end

with ('src/Implementation/MsBwUtility') do |util|
	with ("#{util}/Properties/AssemblyInfo.cs") do |asminfo|
		assemblyinfo :versionmsbswutil do |asm|
			asm.version = "1.0.5"
			asm.file_version = "1.0.5"
			asm.company_name = "BSW Technology Consulting"
			asm.product_name = "MSBW Utility Assembly"
			asm.output_file = asminfo
			asm.input_file = asminfo
		end
	end

	nugetpack :packmsbwutil do |n|
		n.command = ".nuget/nuget.exe"
		n.nuspec = "#{util}/MsBwUtility.csproj"
		n.base_folder = util		
	end
	
	nugetpush :publishmsbwutil do |n|
	end
end
