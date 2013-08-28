@echo off
REM These environment variables set by TeamCity seem to get in the way of bundle install completing properly
set gem_home=
set gem_path=
set rubylib=
set rubyopt=
set bundle_bin_path=
set bundle_gemfile=

REM execute the bundle install

bundle install > bundle_install.log