GitHubFlowVersion
=================

The easy way to use semantic versioning (semver.org) with a [GitHub Flow](http://scottchacon.com/2011/08/31/github-flow.html) or trunk/mainline development Git workflow.

It supports using **any** Git repository as long as you are using master and furthermore, it supports detection and versioning of **Pull Requests** if you are using GitHub or Stash.

*GitHubFlowVersion* will automatically version your application to SemVer of  `{vLast.Major}.{vLast.Minor}.{vLast.Patch+1}` where vLast is the last **Git Tag** in your repo.

This means your versions are based on **source control metadata** making it repeatable. *GitHubFlowVersion* gives you flexibility by making variables available to your build so you can meet all your versioning requirements. See [Available Variables](https://github.com/JakeGinnivan/GitHubFlowVersion/wiki/Available-Variables) for the full list of variables

It also means that unlike many other versioning strategies **you do not have to rebuild your project to bump the version!**

There are two ways to use GitHubFlowVersion

### 1. In TeamCity
Simply call `GitHubFlowVersion.exe [/UpdateAssemblyInfo]` as the first step in your TeamCity build process

This will set the TeamCity build number to `{vLast.Major}.{vLast.Minor}.{vLast.Patch+1}+{###}` (by setting the `%build.number%` variable). For example if the last **tag** was 1.2.0 then TeamCity's build number will be 1.2.1+005 (where 5 is the number of commits since the last tag). Read more about the [Versioning Conventions](https://github.com/JakeGinnivan/GitHubFlowVersion/wiki/Versioning-Conventions)

The [Available Variables](https://github.com/JakeGinnivan/GitHubFlowVersion/wiki/Available-Variables) for the full list of variables will become TeamCity `system.` variables for you to pass/use in the rest of your build process

### 2. Standalone
To make all the variables available, GitHubFlowVersion has to call your build scripts. We support MSBuild or any arbitrary executable (including non-.NET applications) via the `ProjectFile` and `Exec` commandline options respectively:

    GitHubFlowVersion.exe /ProjectFile MyMSBuildProject.proj /Targets Build;Package [/UpdateAssemblyInfo]
    or
    GitHubFlowVersion.exe /Exec rake.exe /ExecArgs "scripts/build" [/UpdateAssemblyInfo]
    
GitHubFlowVersion.exe does this to version itself, See [GitHubFlowVersion's Build Script](https://github.com/JakeGinnivan/GitHubFlowVersion/blob/master/GitHubFlowVersion.proj) for examples on how it can be used in an MSBuild file and 

## How to increase the Semantic Version
To bump the SemVer of your project, simple add/edit the `NextVersion.txt` file and put the next SemVer into it. GitHubFlowVersion will use the version in NextVersion.txt over the convention *if* it is higher than the convention version. Which means when you **tag** your release, then your patch will automatically be bumped for the next build and the build metadata will reset to 000.

## Getting started
You can install it from [NuGet](https://www.nuget.org/packages/GitHubFlowVersion/) or download the compiled executable from the [Latest Release on GitHub](https://github.com/JakeGinnivan/GitHubFlowVersion/releases).

Installing from NuGet will copy itself into `$(SolutionDir)\tools\GitHubFlowVersion`, this gives you a fixed path to run GitHubFlowVersion from which is not affected by package restore.

## FAQ
### Why do I need to call my build process through GitHubFlowVersion
Environmental variables can be set to process or user scoped (which is not an option for us), if you build script calls GitHubFlowVersion it will run then exit, and the variables it sets will not be avaialble for your build

### How do you update AssemblyInfo.cs
If you specify the `/UpdateAssemblyInfo` flag, then we will find all `AssemblyInfo.cs` files in your git repo and update the AssemblyVersion, AssemblyFileVersion and AssemblyInformationalVersion.
We will only update the version in attributes which exist, so if you want us to update the AssemblyInformationalVersion then simply add that attribute into your AssemblyInfo.cs and it will be updated.

If you ran *GitHubFlowVersion* with the /Exec or the /ProjectFile parameters we will automatically revert the changes we made to your AssemblyInfo.cs files after we have run your build.

### How is this different to just using Versions.txt
It isn't that different, except it adds the following which makes the process work for Continuous Deliver and also generally reduces friction

 - After a release is tagged, the patch will be bumped automatically
 - Pull requests are labeled as -PullRequestXX, meaning they have a lower semantic version than `master`
 - Non-master branches will be tagged with the branch name, making their semantic version lower, for example a branch named `beta` will have a SemVer of `1.2.1-beta+000`
 - Each pull request can edit the `NextVersion.txt` file independently, and the CI builds which are produced will all be correct, the `NextVersion.txt` then can be merged and you simply take the highest number

Conceptually it is also different, we are saying the next version to be released is #.#.#. Then all non-master builds are automatically pre-release making their version lower than master.

Read more at [How GitHubVersion works](https://github.com/JakeGinnivan/GitHubFlowVersion/wiki/How-it-works)

### What is the difference between [GitFlowVersion](https://github.com/Particular/GitFlowVersion) and this?
This project is based on the idea's of GitFlowVersion except it is designed to work with `GitHub Flow` rather than `GitFlow` which is a much more complicated branching strategy

### What is the +BuildMetadata
SemVer 2.0 allows you to specify build metadata, which **does not affect** the Semantic Version. For example `1.2.0+011` is the same Semantic Version as `1.2.0+100`  
We use this by default so CI builds consistently build the *next* releasable package

### Tell me more about the pull request and pre-release support
In a nutshell, they will simply follow the same conventions as master, except all builds will have a pre-release tag of `-PullRequest` for pull requests and `-{branchname}` for non-master branches. Read more at [Pull Requests](https://github.com/JakeGinnivan/GitHubFlowVersion/wiki/Pull-Requests) or [Non-master branches](https://github.com/JakeGinnivan/GitHubFlowVersion/wiki/Non-master-branches)

## History
The idea and a bit of the code is from [https://github.com/Particular/GitFlowVersion](https://github.com/Particular/GitFlowVersion).  
I decided not to fork because this project is a lot simpler. If you use GitFlow I highly suggest you check out GitFlowVersion, it is a really great idea.

This project was born because I prefer GitHub Flow over GitFlow for my open source projects.

## Contributions / Issues / Questions

Feedback, contributions, bug reports and questions are more than welcome, please raise issues/suggestions at: https://github.com/JakeGinnivan/GitHubFlowVersion/issues or contact [Jake Ginnivan](https://twitter.com/jakeginnivan), [Robert Moore](https://twitter.com/robdmoore) or [Matt Davies](https://twitter.com/mdaviesnet) on Twitter.
