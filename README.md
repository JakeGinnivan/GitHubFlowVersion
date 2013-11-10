GitHubFlowVersion
=================

The easy way to use semantic versioning (semver.org) with a GitHub Flow or trunk/mainline development Git workflow.

It supports using any Git repository as long as you are using master and furthermore, it supports detection of Pull Requests if you are using:

* GitHub
* Stash

GitHubFlowVersion is an exe which versions your software through a few simple conventions:

 - Current version being build is `{vLast.Major}.{vLast.Minor}.{vLast.Patch+1}`
 - If you want to bump Major or Minor due to changes that are more involved than a patch simply update `NextVersion.txt` which lives in the repository root
 - A bunch of variables are written out to TeamCity for you to use in your build as documented below or, alternatively, GitHubFlowVersion can execute your build scripts making a bunch of environmental variables available (as documented below)

See below for full usage instructions.

GitHubFlowVersion works best if you are using TeamCity (but that's not a requirement) and while it was designed with .NET projects in mind, it can be used for any project type you like as long as you are running it in Windows.

You can install it from https://www.nuget.org/packages/GitHubFlowVersion/ using NuGet or download the compiled executable from https://github.com/JakeGinnivan/GitHubFlowVersion/releases.

We are also starting a project for the next step: releasing the properly versioned files. Get involved at https://github.com/JakeGinnivan/GitHubReleaser.

GitHubFlowVersion builds itself, then uses what it has just built to version itself. This means it is constantly being dogfooded and can be used as an example on how it can be used. Check out the CI builds at [GitHubFlowVersion's TeamCity Server](http://teamcity.ginnivan.net/project.html?projectId=OpenSourceProjects_GitHubFlowVersion&branch_OpenSourceProjects_GitHubFlowVersion=__all_branches__).

### History
The idea and a bit of the code is from [https://github.com/Particular/GitFlowVersion](https://github.com/Particular/GitFlowVersion).  
I decided not to fork because this project is a lot simpler. If you use GitFlow I highly suggest you check out GitFlowVersion, it is a really great idea.

This project was born because I prefer GitHub Flow over GitFlow for my open source projects.

### How GitHubFlowVersion works

If you don't know what GitHub Flow is there is a good write up at http://scottchacon.com/2011/08/31/github-flow.html.

In a nutshell though, GitHub Flow involves the following steps:

* Fork the main repository
* Clone your fork
* Locally you branch from master
* Commit your work
* Push your branch to your fork
* Submit a pull request from your fork's branch to master of the main repository

GitHubFlowVersion simply requires you to tag master when you release (known as **vLast** throughout the rest of this document), and add a `NextVersion.txt` file to control your versions using the following simple rules:

 * If `NextVersion.txt` is a lower or the same SemVer as vLast, the build SemVer will be `{vLast.Major}.{vLast.Minor}.{vLast.Patch+1}`
   * This means you don't have to update the `NextVersion.txt` file when you release, but you can bump it whenever you need to  increase by more than a patch
 * If `NextVersion.txt` is a higher SemVer to the last tag the version in `NextVersion.txt` is used
 * If you have not ever created a tag (new project), GitHubFlowVersion will create the `NextVersion.txt` for you at v0.1.0

This solves the major SemVer issue of tying the SemVer to your code without having to constantly change a file. This SemVer (known as **NextSemVer** throughout the rest of this document) won't change between multiple commits to the branch though (until you either release and tag the branch or change the `NextVerison.txt` file), but we need to be able to version all our builds with a unique version. The approach GitHubFlowVersion uses to solve this is described in the next section.

### Versioning Conventions
All builds on the same branch as the last vLast will have the format:

    {NextSemVer}+{commitsSince_vLast}

For example, if I tag a release 0.2.0 (so the tag is on the HEAD of master) and I merge a pull request with 2 commits then the build number will be: `0.2.1+003`.

This convention gives you a number of advantages:

* You can use continuous delivery with the trigger of a version bump being after the build (say when you decide to promote to production) without having to re-build the software (thus following the principle of compiling once and using that result throughout the rest of the deployment pipeline).
    * This is because the build metadata has no semantic meaning so `0.2.1+003` is the same semantic version as `0.2.1+234` - thus you can decide to deploy the software at any time and your semantic version number will have increased by one increment only (thus conforming to SemVer rules)
* You have a unique build identifier for every push to master
* You are not relying on the CI server to generate your unique identifier - you can completely rebuild the CI server and point it at a commit and it will generate the same build identifier (i.e. it's idempotent)
* All versioning information is kept in source control
* You get an indication of how long it has been since you last released (in terms of number of commits), which provides a handy hint to encourage you to release more often (when the number gets higher your confidence level about whether there is a bug in the release decreases)

For situations where you need a four part version number there is a four part variable exposed by GitHubFlowVersion that uses the number of commits as a fourth number rather than metadata, e.g. `0.2.1.3` - see below for the variable name.

#### Pull Requests
If you are using GitHub or Stash then pull requests will be automatically tagged with a pre-release identifier. BitBucket is not supported because of [https://bitbucket.org/site/master/issue/5814/reify-pull-requests-by-making-them-a-ref](https://bitbucket.org/site/master/issue/5814/reify-pull-requests-by-making-them-a-ref).

For example: If you have a pull request that branched away from master after a tag of `0.2.0` and there has been 3 commits since that release and the pull request number in the repository is 25 then the build identifier `0.2.1-PullRequest25+003`.

The number of commits since the last release is added to the build metadata here because you can push further commits to the pull request so this is important to keep the build identifiers unique.

#### Non-master branches
Any other branches in your repository will use the branch name as a pre-release tag. For example if you create a branch called `beta`, then it will have a version `0.2.1-beta+003` if it was branched from a point where master has a tag of `0.2.0` and there has been 3 commits since.

This allows you to have short lived branches for a pre-release, improve it. Then merge it back into master when you are done, then release the final build.

## How to use it

There is two main approaches to use GitHubFlowVersion - TeamCity and Local Builds.

### TeamCity
Simply run `GitHubFlowVersion.exe` as the first step of your build and it will set the `%build.number%` variable in TeamCity to the `FullSemVer` (see below).
then all the variables above will be available as `system` variables so they will automatically be passed to any build system you use.
Calling GitHubFlowVersion.exe from TeamCity will cause a bunch of system variables to be created in TeamCity (as specified below)and it will also override the `%build.number%`.

**For the moment you need to promote the `%teamcity.build.vcs.branch.{configurationid}%` build parameter to an environment variable called `env.GitBranchName` for pull request detection to work.**

### Local Builds (or other build servers)
To make all the variables available, GitHubFlowVersion has to call your build scripts. We support MSBuild or any arbitrary executable (including non-.NET applications) via the `ProjectFile` and `Exec` commandline options respectively:

        GitHubFlowVersion.exe /ProjectFile MyMSBuildProject.proj /Targets Build;Package
        GitHubFlowVersion.exe /Exec rake.exe /ExecArgs "scripts/build" (you don't have to call MSBuild - this is just a convenient example)

The variables will be available to your build as Environmental Variables (as specified below).

See [GitHubFlowVersion's Build Script](https://github.com/JakeGinnivan/GitHubFlowVersion/blob/master/GitHubFlowVersion.proj) for examples on how it can be used in an MSBuild file.

GitHubFlowVersion has to be the one which executes your build system because process level environmental variables will not be available to the parent process, but they will be available to child processes.

### Available Variables

**Note:** The variable names will have the `_` replaced with a `.` in TeamCity.

* Without tag (on master)
    * GitHubFlowVersion_FullSemVer (e.g. `1.2.3+004`)
    * GitHubFlowVersion_SemVer (e.g. `1.2.3`)
    * GitHubFlowVersion_AssemblySemVer (e.g. `1.2.3.0`)
    * GitHubFlowVersion_FourPartVersion (e.g. `1.2.3.4`)
    * GitHubFlowVersion_NumCommitsSinceRelease (e.g. `4`)
    * GitHubFlowVersion_Major (e.g. `1`)
    * GitHubFlowVersion_Minor (e.g. `2`)
    * GitHubFlowVersion_Patch (e.g. `3`)
    * GitHubFlowVersion_NumCommitsSinceRelease (e.g. `4`)
    * GitHubFlowVersion_Tag (e.g. ``)
* With tag (on a branch)
    * GitHubFlowVersion_FullSemVer (e.g. `1.2.3-alpha+004`)
    * GitHubFlowVersion_SemVer (e.g. `1.2.3-alpha`)
    * GitHubFlowVersion_AssemblySemVer (e.g. `1.2.3.0`)
    * GitHubFlowVersion_FourPartVersion (e.g. `1.2.3.4`)
    * GitHubFlowVersion_NumCommitsSinceRelease (e.g. `4`)
    * GitHubFlowVersion_Major (e.g. `1`)
    * GitHubFlowVersion_Minor (e.g. `2`)
    * GitHubFlowVersion_Patch (e.g. `3`)
    * GitHubFlowVersion_NumCommitsSinceRelease (e.g. `4`)
    * GitHubFlowVersion_Tag (e.g. `alpha`)
* With tag (on a pull request branch)
    * GitHubFlowVersion_FullSemVer (e.g. `1.2.3-PullRequest20+004`)
    * GitHubFlowVersion_SemVer (e.g. `1.2.3-PullRequest20`)
    * GitHubFlowVersion_AssemblySemVer (e.g. `1.2.3.0`)
    * GitHubFlowVersion_FourPartVersion (e.g. `1.2.3.4`)
    * GitHubFlowVersion_NumCommitsSinceRelease (e.g. `4`)
    * GitHubFlowVersion_Major (e.g. `1`)
    * GitHubFlowVersion_Minor (e.g. `2`)
    * GitHubFlowVersion_Patch (e.g. `3`)
    * GitHubFlowVersion_NumCommitsSinceRelease (e.g. `4`)
    * GitHubFlowVersion_Tag (e.g. `PullRequest20`)

Further documentation about these variables is available at https://github.com/JakeGinnivan/GitHubFlowVersion/blob/master/src/GitHubFlowVersion.Tests/VariableProviderTests.cs (the variable provider test).

### How do I patch assembly info?
I would suggest using [https://github.com/loresoft/msbuildtasks](https://github.com/loresoft/msbuildtasks) in your MSBuild script, a sample is available at [https://github.com/loresoft/msbuildtasks/blob/master/Source/Sample.proj](https://github.com/loresoft/msbuildtasks/blob/master/Source/Sample.proj) or you can see how it's been done for [GitHubFlowVersion](https://github.com/JakeGinnivan/GitHubFlowVersion/blob/master/GitHubFlowVersion.proj#L32).

If you would like built in AssemblyInfo patching support please discuss it in the [relevant issue](https://github.com/JakeGinnivan/GitHubFlowVersion/issues/23).

## Contributions / Issues / Questions

Feedback, contributions, bug reports and questions are more than welcome, please raise issues/suggestions at: https://github.com/JakeGinnivan/GitHubFlowVersion/issues or contact [Jake Ginnivan](https://twitter.com/jakeginnivan), [Robert Moore](https://twitter.com/robdmoore) or [Matt Davies](https://twitter.com/mdaviesnet) on Twitter.