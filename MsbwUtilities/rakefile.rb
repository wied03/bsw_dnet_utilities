$: << File.expand_path(File.dirname(__FILE__))
require "msbuild"
require "tools"
require "config"
require "nunit"

with('MsbwUtilities.sln') do |sln|
	BradyW::MSBuild.new :clean do |clean|
		clean.targets = "clean"
		clean.solution = sln
	end

	BradyW::MSBuild.new :build do |build|
		build.solution = sln
	end
end

task :ci => [:clean, :build, :test]

task :test => [:codetest]

with ('test') do |t|	
	BradyW::Nunit.new :codetest => :build do |test|
		test.files = FileList["#{t}/**/bin/Debug/*Test.dll"]
		# Since we have some expensive tests and our box is slow, let's do a timeout of 60 seconds
		test.timeout = 60000
	end	
end