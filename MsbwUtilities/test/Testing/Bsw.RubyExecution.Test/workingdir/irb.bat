@echo off
REM Send the args to a file so we can read it and assert the correct parameters
echo %* > command_executed.txt