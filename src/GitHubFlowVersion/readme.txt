GitHubFlowVersion
-----------------

So you want to be easily able to do SemVer with your project, this project is designed to reduce friction and make it easy to do SemVer as well as Continuous Integration or Delivery.

The idea is you tag releases, then the subsiquent builds have the patch version increased by 1. 
If you want to bump the major or minor, simply update NextVersion.txt (created in the repository root) and it will be used as the base SemVer.

Usage
-----

There is two main approaches to use GitHubFlowVersion

1. Build Server
Calling GitHubFlowVersion.exe from TeamCity will cause a bunch of system variables to be created in TeamCity, GitHubFlowVersion will also override the Build.Number

2. Local Builds
To make all the variables available, GitHubFlowVersion has to call your build. We support MSBuild and also can execute any process (meaning this can work with non-dotnet applications!)

GitHubFlowVersion.exe /ProjectFile MyMSBuildProject.proj /Targets Build;Package
GitHubFlowVersion.exe /Exec msbuild.exe /ExecArgs "MyMSBuildProject.proj /Targets:Build;Package" (you can't have to call MSBuild)

The variables will be available to your build as Environmental Variables


Available Variables
-------------------
For examples of what the variables would be in different scenarios, checkout the tests at

https://github.com/JakeGinnivan/GitHubFlowVersion/blob/master/src/GitHubFlowVersion.Tests/VariableProviderTests.cs

TeamCity variables will have _ replaced with . to follow the same conventions


Feedback welcome, please raise issues/suggestions at
https://github.com/JakeGinnivan/GitHubFlowVersion/issues