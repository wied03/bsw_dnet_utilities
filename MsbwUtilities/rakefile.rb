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

task :version => [:versionbswutil,
				  :versionbswtest,
				  :versionbaseentities,
				  :versionnhibutils,
				  :versionrubyexecution,
				  :versionnhibtest]
				  
task :pack => [:packbswutil,
			   :packbswtest,
			   :packbaseentities,
			   :packnhibutils,
			   :packrubyexecution,
			   :packnhibtest]

with ('test') do |t|	
	BradyW::Nunit.new :codetest => :build do |test|
		test.files = FileList["#{t}/**/bin/Debug/*Test.dll"]
	end	
end

task :cleanpackages do
	rm_rf FileList['**/*.nupkg']
end

with (".nuget/nuget.exe") do |ngetpath|
	with (ENV['version_number']) do |ver|
		with("BSW Technology Consulting") do |companyName|

			with ('src/Implementation/Bsw.NHibernateUtils') do |projPath|
				with ("#{projPath}/Properties/AssemblyInfo.cs") do |asminfo|
					assemblyinfo :versionnhibutils do |asm|
						puts "Putting version number #{ver} on assembly"
						asm.version = ver
						asm.file_version = ver
						asm.company_name = companyName
						asm.product_name = "BSW NHibernate Utilities"
						asm.output_file = asminfo
						asm.input_file = asminfo
					end			
				end
				
				nugetpack :packnhibutils do |n|
						n.command = ngetpath
						n.nuspec = "#{projPath}/Bsw.NHibernateUtils.csproj"
						n.base_folder = projPath
						n.output = projPath
				end					
			end
			
			with ('src/Testing/Bsw.NHibernate.Testing') do |projPath|
				with ("#{projPath}/Properties/AssemblyInfo.cs") do |asminfo|
					assemblyinfo :versionnhibtest do |asm|
						puts "Putting version number #{ver} on assembly"
						asm.version = ver
						asm.file_version = ver
						asm.company_name = companyName
						asm.product_name = "BSW NHibernate Testing"
						asm.output_file = asminfo
						asm.input_file = asminfo
					end			
				end
				
				nugetpack :packnhibtest do |n|
						n.command = ngetpath
						n.nuspec = "#{projPath}/Bsw.NHibernate.Testing.csproj"
						n.base_folder = projPath
						n.output = projPath
				end					
			end
		
			with ('src/Implementation/Bsw.BaseEntities') do |projPath|
				with ("#{projPath}/Properties/AssemblyInfo.cs") do |asminfo|
					assemblyinfo :versionbaseentities do |asm|
						puts "Putting version number #{ver} on assembly"
						asm.version = ver
						asm.file_version = ver
						asm.company_name = companyName
						asm.product_name = "BSW Base NHibernate Entities"
						asm.output_file = asminfo
						asm.input_file = asminfo
					end			
				end
				
				nugetpack :packbaseentities do |n|
						n.command = ngetpath
						n.nuspec = "#{projPath}/Bsw.BaseEntities.csproj"
						n.base_folder = projPath
						n.output = projPath
				end					
			end
			
			with ('src/Testing/Bsw.RubyExecution') do |projPath|
				with ("#{projPath}/Properties/AssemblyInfo.cs") do |asminfo|
					assemblyinfo :versionrubyexecution do |asm|
						puts "Putting version number #{ver} on assembly"
						asm.version = ver
						asm.file_version = ver
						asm.company_name = companyName
						asm.product_name = "BSW Ruby Execution"
						asm.output_file = asminfo
						asm.input_file = asminfo
					end			
				end
				
				nugetpack :packrubyexecution do |n|
						n.command = ngetpath
						n.nuspec = "#{projPath}/Bsw.RubyExecution.csproj"
						n.base_folder = projPath
						n.output = projPath
				end					
			end
			
			with ('src/Testing/MsbwTest') do |projPath|
				with ("#{projPath}/Properties/AssemblyInfo.cs") do |asminfo|			
					assemblyinfo :versionbswtest do |asm|
						puts "Putting version number #{ver} on assembly"
						asm.version = ver
						asm.file_version = ver
						asm.company_name = companyName
						asm.product_name = "MSBW Test Assembly"
						asm.output_file = asminfo
						asm.input_file = asminfo
					end
				end
				
				nugetpack :packbswtest do |n|
					n.command = ngetpath
					n.nuspec = "#{projPath}/MsbwTest.csproj"
					n.base_folder = projPath
					n.output = projPath
				end				
			end

			with ('src/Implementation/MsBwUtility') do |projPath|
				with ("#{projPath}/Properties/AssemblyInfo.cs") do |asminfo|			
					assemblyinfo :versionbswutil do |asm|
						puts "Putting version number #{ver} on assembly"
						asm.version = ver
						asm.file_version = ver
						asm.company_name = companyName
						asm.product_name = "MSBW Utility Assembly"
						asm.output_file = asminfo
						asm.input_file = asminfo
					end
				end
				
				nugetpack :packbswutil do |n|
					n.command = ngetpath
					n.nuspec = "#{projPath}/MsBwUtility.csproj"
					n.base_folder = projPath
					n.output = projPath
				end				
			end
		end
	end
end
