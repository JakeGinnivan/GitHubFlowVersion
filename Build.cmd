@ECHO OFF

SET msbuild="%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"

%msbuild% GitHubFlowVersion.proj /target:Cleanup
%msbuild% %~dp0\src\GitHubFlowVersion.sln
"%~dp0\src\GitHubFlowVersion\bin\Debug\GitHubFlowVersion.exe" /w "%~dp0\" /ProjectFile GitHubFlowVersion.proj /Targets Publish

IF NOT ERRORLEVEL 0 EXIT /B %ERRORLEVEL%

pause