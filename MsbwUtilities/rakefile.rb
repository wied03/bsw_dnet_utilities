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
# Version here because our re-build below with forcebuildforpackages will not execute for each package
task :package => [:clean, :version, :pack]

task :version => [:versionbswutil,
				  :versionbswtest,
				  :versionbaseentities,
				  :versionnhibutils,
				  :versionrubyexecution,
				  :versionnhibtest]

# Our re-build below with forcebuildforpackages will not execute for each package
task :push => [:package,
				:pushbswutil,
				  :pushbswtest,
				  :pushbaseentities,
				  :pushnhibutils,
				  :pushrubyexecution,
				  :pushnhibtest]				  
				  
# We might have already done this in this build cycle, but we update the source with versions
# so need to do force a build
task :forcebuildforpackages do
	Rake::Task["build"].execute
end
				  
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
	with (ENV['nuget_apikey']) do |apikey|
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
					
					nugetpack :packnhibutils => [:versionnhibutils,:forcebuildforpackages] do |n|
							n.command = ngetpath
							n.nuspec = "#{projPath}/Bsw.NHibernateUtils.csproj"
							n.base_folder = projPath
							n.output = projPath
					end

					nugetpush :pushnhibutils => :packnhibutils do |n|
						n.command = ngetpath
						n.package = "#{projPath}/Bsw.NHibernateUtils.#{ver}.nupkg"
						n.apikey = apikey						
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
					
					nugetpack :packnhibtest => [:versionnhibtest,:forcebuildforpackages] do |n|
							n.command = ngetpath
							n.nuspec = "#{projPath}/Bsw.NHibernate.Testing.csproj"
							n.base_folder = projPath
							n.output = projPath
					end

					nugetpush :pushnhibtest => :packnhibtest do |n|
						n.command = ngetpath
						n.package = "#{projPath}/Bsw.NHibernate.Testing.#{ver}.nupkg"
						n.apikey = apikey						
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
					
					nugetpack :packbaseentities => [:versionbaseentities,:forcebuildforpackages] do |n|
							n.command = ngetpath
							n.nuspec = "#{projPath}/Bsw.BaseEntities.csproj"
							n.base_folder = projPath
							n.output = projPath
					end

					nugetpush :pushbaseentities => :packbaseentities do |n|
						n.command = ngetpath
						n.package = "#{projPath}/Bsw.BaseEntities.#{ver}.nupkg"
						n.apikey = apikey						
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
					
					nugetpack :packrubyexecution => [:versionrubyexecution,:forcebuildforpackages] do |n|
							n.command = ngetpath
							n.nuspec = "#{projPath}/Bsw.RubyExecution.csproj"
							n.base_folder = projPath
							n.output = projPath
					end

					nugetpush :pushrubyexecution => :packrubyexecution do |n|
						n.command = ngetpath
						n.package = "#{projPath}/Bsw.RubyExecution.#{ver}.nupkg"
						n.apikey = apikey						
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
					
					nugetpack :packbswtest => [:versionbswtest,:forcebuildforpackages] do |n|
						n.command = ngetpath
						n.nuspec = "#{projPath}/MsbwTest.csproj"
						n.base_folder = projPath
						n.output = projPath
					end

					nugetpush :pushbswtest => :packbswtest do |n|
						n.command = ngetpath
						n.package = "#{projPath}/MsbwTest.#{ver}.nupkg"
						n.apikey = apikey						
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
					
					nugetpack :packbswutil => [:versionbswutil,:forcebuildforpackages] do |n|
						n.command = ngetpath
						n.nuspec = "#{projPath}/MsBwUtility.csproj"
						n.base_folder = projPath
						n.output = projPath
					end
					
					nugetpush :pushbswutil => :packbswutil do |n|
						n.command = ngetpath
						n.package = "#{projPath}/MsBwUtility.#{ver}.nupkg"
						n.apikey = apikey						
					end					
				end
			end
		end
	end
end
