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

task :version => [:versionbswutil, :versionbswtest]
task :pack => [:packbswutil, :packbswtest]

with ('test') do |t|	
	BradyW::Nunit.new :codetest => :build do |test|
		test.files = FileList["#{t}/**/bin/Debug/*Test.dll"]
	end	
end

task :cleanpackages do
	rm_rf FileList['**/*.nupkg']
end

with (ENV['version_number']) do |ver|
	with ('src/Testing/MsbwTest') do |projpath|
		with ("#{projpath}/Properties/AssemblyInfo.cs") do |asminfo|			
			assemblyinfo :versionbswtest do |asm|
				puts "Putting version number #{ver} on assembly"
				asm.version = ver
				asm.file_version = ver
				asm.company_name = "BSW Technology Consulting"
				asm.product_name = "MSBW Test Assembly"
				asm.output_file = asminfo
				asm.input_file = asminfo
			end
		end

		with (".nuget/nuget.exe") do |ngetpath|
			nugetpack :packbswtest do |n|
				n.command = ngetpath
				n.nuspec = "#{projpath}/MsbwTest.csproj"
				n.base_folder = projpath
				n.output = projpath
			end		
		end
	end

	with ('src/Implementation/MsBwUtility') do |projpath|
		with ("#{projpath}/Properties/AssemblyInfo.cs") do |asminfo|			
			assemblyinfo :versionbswutil do |asm|
				puts "Putting version number #{ver} on assembly"
				asm.version = ver
				asm.file_version = ver
				asm.company_name = "BSW Technology Consulting"
				asm.product_name = "MSBW Utility Assembly"
				asm.output_file = asminfo
				asm.input_file = asminfo
			end
		end

		with (".nuget/nuget.exe") do |ngetpath|
			nugetpack :packbswutil do |n|
				n.command = ngetpath
				n.nuspec = "#{projpath}/MsBwUtility.csproj"
				n.base_folder = projpath
				n.output = projpath
			end		
		end
	end
end
