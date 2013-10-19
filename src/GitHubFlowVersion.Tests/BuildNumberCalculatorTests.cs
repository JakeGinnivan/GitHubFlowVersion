using System;
using LibGit2Sharp;
using NSubstitute;
using Xunit;

namespace GitHubFlowVersion.Tests
{
    public class BuildNumberCalculatorTests
    {
        private readonly INextSemverCalculator _nextSemver;
        private ILastTaggedReleaseFinder _lastTaggedReleaseFinder;
        private readonly IRepository _gitRepo;
        private readonly BuildNumberCalculator _sut;
        private IGitHelper _gitHelper;

        public BuildNumberCalculatorTests()
        {
            _nextSemver = Substitute.For<INextSemverCalculator>();
            _lastTaggedReleaseFinder = Substitute.For<ILastTaggedReleaseFinder>();
            _gitRepo = Substitute.For<IRepository>();
            _gitHelper = Substitute.For<IGitHelper>();
            _sut = new BuildNumberCalculator(_nextSemver, _lastTaggedReleaseFinder, _gitHelper, _gitRepo);
        }

        [Fact]
        public void ReturnsSameSemanticVersionAsNextVersionCalculator()
        {
            _lastTaggedReleaseFinder.GetVersion().Returns(new VersionTaggedCommit(null, null));
            var semver = new SemanticVersion(0, 1, 2);
            _nextSemver.NextVersion().Returns(semver);

            var buildNumber = _sut.GetBuildNumber();

            Assert.Equal(semver, buildNumber);
        }

        [Fact]
        public void AppendsBuildOfCommitsSinceLastReleaseAsBuildMetaData()
        {
            _lastTaggedReleaseFinder.GetVersion().Returns(new VersionTaggedCommit(null, null));
            _gitHelper.NumberOfCommitsOnBranchSinceCommit(Arg.Any<Branch>(), Arg.Any<Commit>()).Returns(5);
            var semver = new SemanticVersion(0, 1, 2);
            _nextSemver.NextVersion().Returns(semver);

            var buildNumber = _sut.GetBuildNumber();

            Assert.Equal("0.1.2+005", buildNumber.ToString());
        }

        [Fact]
        public void PullRequestsAreTaggedWithPreReleaseSemverTag()
        {
            _lastTaggedReleaseFinder.GetVersion().Returns(new VersionTaggedCommit(null, null));
            _gitHelper.IsPullRequest(Arg.Any<Branch>()).Returns(true);
            _gitHelper.NumberOfCommitsOnBranchSinceCommit(Arg.Any<Branch>(), Arg.Any<Commit>()).Returns(5);
            var semver = new SemanticVersion(0, 1, 2);
            _nextSemver.NextVersion().Returns(semver);

            var buildNumber = _sut.GetBuildNumber();

            Assert.Equal("0.1.2-PullRequest+005", buildNumber.ToString());
        }
    }
}