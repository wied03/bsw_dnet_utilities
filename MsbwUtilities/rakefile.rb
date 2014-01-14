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
# Our re-build below with forcebuildforpackages will not execute for each package
task :push => [:package]				  
				  
# We might have already done this in this build cycle, but we update the source with versions
# so need to do force a build
task :forcebuildforpackages do
	Rake::Task["build"].execute
end

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
			define_project = lambda do |options|					
					project_path = options[:project_path]
					# derive the project name from the path (convention over config)
					project_name = File.basename(project_path)					
					project_title = options[:project_title]
					proj_path_and_name = "#{project_path}/#{project_name}"
					specTask = "spec_#{project_name}"
					desc "Builds nuspec for #{project_name}"
					nuspec specTask do |n|
						n.id = project_name
						n.title = project_title
						n.version = ver
						n.output_file = "#{proj_path_and_name}.nuspec"
						n.authors = companyName
						n.owners = n.authors
						n.require_license_acceptance = "false"
						n.description = options[:project_description]
						n.projectUrl= "https://github.com/wied03/bsw_dnet_utilities"
						n.copyright = "Copyright #{Date.today.year}"
						if options.has_key?(:dependencies)
							options[:dependencies].each {|d|  n.dependency d[:id], d[:version]}
						end
					end
				
					asmTask = "asm_#{project_name}"
					desc "Updates assembly info for #{project_name}"
					with ("#{project_path}/Properties/AssemblyInfo.cs") do |asminfo|			
						assemblyinfo asmTask do |asm|
							puts "Putting version number #{ver} on assembly"
							asm.version = ver
							asm.file_version = ver
							asm.company_name = companyName
							asm.product_name = project_title
							asm.output_file = asminfo
							asm.input_file = asminfo
						end
					end
					
					packTask = "pack_#{project_name}"
					desc "Creates a nupkg file for #{project_name}"
					nugetpack packTask => [specTask,asmTask,:forcebuildforpackages] do |n|
						#n.log_level = :verbose
						n.command = ngetpath
						n.nuspec = "#{proj_path_and_name}.csproj"
						n.base_folder = project_path
						n.output = project_path						
					end					
					
					pushTask = "push_#{project_name}"
					desc "Pushes the nuget package to nuget.org for #{project_name}"
					nugetpush pushTask => packTask do |n|
						n.command = ngetpath
						n.package = "#{proj_path_and_name}.#{ver}.nupkg"
						n.apikey = apikey						
					end										
					
					task :version => asmTask
					task :pack => packTask
					task :push => pushTask								
			end
			
				define_project.call  :project_path => 'src/Implementation/Bsw.NHibernateUtils',
					 :project_title => 'BSW NHibernate Utilities',
					 :project_description => "Provides unit of work, StructureMap dependency injection base registry, and base repository for building a data layer"							
				define_project.call  :project_path => "src/Testing/Bsw.NHibernate.Testing",
					 :project_title => 'BSW NHibernate Testing Utility',
					 :project_description => "Provides convenience code for doing SQLite in memory unit testing with NHibernate"				

				define_project.call  :project_path => "src/Implementation/Bsw.BaseEntities",
					 :project_title => 'BSW Base Entities',
					 :project_description => "Provides base NHibernate entity classes that take care of equality, etc."
				
				define_project.call  :project_path => "src/Testing/Bsw.RubyExecution",
					 :project_title => 'BSW Ruby Execution Utility',
					 :project_description => "Starts/stops a Ruby process (with bundler install support) for use in NUnit tests"
				
				define_project.call  :project_path => "src/Implementation/MsBwUtility",
					 :project_title => 'Msbw Utility Assembly',
					 :project_description => "Used as a utility assembly for implementations"				

				define_project.call :project_path => "src/Testing/MsbwTest",
					 :project_title => 'Msbw Test Assembly',
					 :project_description => "Used as a utility assembly for UNIT TESTING ONLY",
					:dependencies => [:id => 'MsbwUtility', :version => ver]
				
				define_project.call :project_path => "src/Implementation/MsBwUtility",
					 :project_title => 'Msbw Utility Assembly',
					 :project_description => "Used as a utility assembly for implementations"
				
				define_project.call :project_path => "src/Implementation/Bsw.Wpf.Utilities",
					 :project_title => 'BSW WPF Base Utilities',
					 :project_description => "Provides WPF extensions to Prism and services that can be injected into view models"
					 
				define_project.call :project_path => "src/Testing/Bsw.Wpf.Testing.Utilities",
					 :project_title => 'BSW WPF Testing Utilities',
					 :project_description => "Classes to support unit testing WPF applications, in particular those built with Bsw.Wpf.Utilities",
					 :dependencies => [{:id => 'Bsw.Wpf.Utilities', :version => ver},
												 {:id => 'MsbwUtility', :version => ver}]
												 
				 define_project.call :project_path => "src/Testing/Bsw.Utilities.Windows.SystemTest",
					 :project_title => 'BSW WPF System Testing Utilities',
					 :project_description => "Specflow step definitions to support system testing WPF applications with the White framework",
					 :dependencies => [{:id => 'MsbwTest', :version => ver},
												 {:id => 'MsbwUtility', :version => ver}]
			end
		end
	end
end
