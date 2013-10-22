GitHubFlowVersion
=================

The easy way to use semantic versioning (semver.org) with GitHub Flow

The idea and a lot of the heavy lifting code is from [https://github.com/Particular/GitFlowVersion](https://github.com/Particular/GitFlowVersion).  
I decided not to fork because this project will be potentially a lot simpler (hopefully)

If you use GitFlow I highly suggest you check out GitFlowVersion, it is a really great idea

### How GitHubFlowVersion works

If you don't know what GitHub flow is, there is a good write up on http://scottchacon.com/2011/08/31/github-flow.html

But in a nutshell, GitHub flow is: you fork the main repository, then clone your fork. Locally you branch from master, do your work, push your feature branch to your fork, then submit a pull request.  


GitHubFlowVersion simply requires you to tag master when you release (Known as **vLast**), and a NextVersion.txt file to control your versions using the following simple rules

 - If `NextVersion.txt` is a lower or the same SemVer as vLast, the build version will be `{vLast.Major}.{vLast.Minor}.{vLast.Patch+1}`
   - This means you don't have to update the NextVersion.txt file when you release, you can bump it whenever you need to
 - If `NextVersion.txt` is a higher SemVer to the last tag the version in `NextVersion.txt` is used

This solves the major SemVer issues, but now we need to be able to version all our builds with a unique version, read the next section for the rest of the details

### Versioning Conventions
All builds on the same branch as the last SemVer tag (vLast) will have the format:

    {semver}+{commitsSinceLastRelease}

For example, I tag a release 0.2.0 (so the tag is on the HEAD of master), then I merge a pull request with 2 commits, the build number will be:  
`0.2.1+003`

The build metadata (+003) has no semantic meaning in SemVer, so version `0.2.1+003` == `0.2.1+100`, this is good for releases because you can commit multiple times producing the same semantic version, but a different build number.

This is bad for CI (exposing say a TeamCity NuGet feed) because each build is the same version, so you will not be able to just upgrade between CI Builds.

If you need this, the `GitHubFlowVersion.FourPartVersionNumber` variable is available, which will produce `0.2.1.3` instead of `0.2.1+003`. 

#### Pull Requests
Pull requests are automatically tagged as pre-release, for example:

`0.2.1-PullRequest25+003` where there has been 3 commits since the last release (this is because pull requests can be added to, so we need the build metadata to make sure build numbers are unique).

#### Non-master branches
Any other branches in your repo will use the branch name as a pre-release tag. For example if I create a branch called `beta`, then it will have a version `0.2.1-beta+003`.

This allows you to have short lived branches for a pre-release, improve it. Then merge it back into master when you are done, then release the final build.

## How to use it
GitHubFlow version at the moment is designed to work with TeamCity and be as simple as possible, so it is an exe and writes the build number out to TeamCity.

There are a few ways to use GitHubFlowVersion, all involve making a bunch of variables available to you. They are

GitHubFlowVersion.FullSemVer, GitHubFlowVersion.SemVer, GitHubFlowVersion.FourPartVersion  
GitHubFlowVersion.Major, GitHubFlowVersion.Minor, GitHubFlowVersion.Patch  
GitHubFlowVersion.NumCommitsSinceRelease, GitHubFlowVersion.Tag

Documentation about these variables is available at https://github.com/JakeGinnivan/GitHubFlowVersion/blob/master/src/GitHubFlowVersion.Tests/VariableProviderTests.cs (the variable provider test)

### 1. Through TeamCity
Just run `GitHubFlowVersion.exe` as the first step of your build, it will set the build number in teamcity to the FullSemVer
then all the variables above will be available as `system` variables so they will automatically be passed to any build system you use.

For the moment you need to promote the %teamcity.build.vcs.branch.{configurationid}% build parameter to an environment variable with the same name for pull requests to be handled correctly

### 2. Environmental Variables through GitHubFlowVersion (In progress)
The second option which will work for non-dotnet/msbuild/teamcity configurations is to use the `-Exec` or `-Project` switches

#### -Exec
This will set the above variables as process level environmental variables, then will run the command line (exec) or the msbuild project file (-Project).

GitHubFlowVersion has to be the one which executes your build system because process level environmental variabes will not be available to the parent process, but they will be available to child proccesses.


### How do I patch assembly info?
I would suggest using [https://github.com/loresoft/msbuildtasks](https://github.com/loresoft/msbuildtasks) in your MSBuild script, a sample is available at [https://github.com/loresoft/msbuildtasks/blob/master/Source/Sample.proj](https://github.com/loresoft/msbuildtasks/blob/master/Source/Sample.proj)

If you would like built in AssemblyInfo patching support, raise an issue, lets discuss the best way to do it.

## Summary
As you can see, this is a really simple workflow, very easy to get started.
