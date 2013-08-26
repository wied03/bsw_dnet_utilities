@echo off
rem IRB is used so we can get output, otherwise we can't see the output in our .NET unit tests
SET SHUTDOWN=%1
SET SCRIPT_PATH=%2
SHIFT
SHIFT
set after1=

:loop
if "%1" == "" goto end
set after1=%after1% %1
SHIFT
goto loop

:end
echo Current path is %PATH%
rem Bundler gets in the way of redirecting standard out and isn't needed for what we're doing here
set gem_home=
set gem_path=
set rubylib=
set rubyopt=
set bundle_bin_path=
set bundle_gemfile=

echo on 

call irb %SCRIPT_PATH% %after1%
