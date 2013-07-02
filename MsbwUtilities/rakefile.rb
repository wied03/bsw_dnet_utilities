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
	end	
end

task :cleanpackages do
	rm_rf FileList['**/*.nupkg']
end

with (ENV['version_number']) do |ver|
	with ('src/Implementation/MsBwUtility') do |util|
		with ("#{util}/Properties/AssemblyInfo.cs") do |asminfo|		
			assemblyinfo :versionmsbswutil do |asm|
				asm.version = ver
				asm.file_version = ver
				asm.company_name = "BSW Technology Consulting"
				asm.product_name = "MSBW Utility Assembly"
				asm.output_file = asminfo
				asm.input_file = asminfo
			end
		end

		with (".nuget/nuget.exe") do |ngetpath|
			nugetpack :packmsbwutil do |n|
				n.command = ngetpath
				n.nuspec = "#{util}/MsBwUtility.csproj"
				n.base_folder = util
				n.output = util
			end
			
			nugetpush :publishmsbwutil do |n|		
				n.command = ngetpath
				n.log_level = :verbose
				n.package = FileList["#{util}/*.nupkg"]
				# TODO: Set these values from configuration
				n.source = "http://weez.weez.wied.us:8111/httpAuth/app/nuget/v1/FeedService.svc/"
				n.apikey = "3a1cf7ec-788b-44bf-8e52-8d2d18217e56"
			end
		end
	end
end
