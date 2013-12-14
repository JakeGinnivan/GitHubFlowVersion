using GitVersion;
using GitVersion.BranchingStrategies.GitHubFlow;
using GitVersion.BuildServers;
using GitVersion.Infrastructure;
using LibGit2Sharp;
using NSubstitute;
using Xunit;

namespace GitHubFlowVersion.Tests
{
    public class BuildNumberCalculatorTests
    {
        private readonly ISemanticVersionCalculator _semanticVersion;
        private readonly ILastTaggedReleaseFinder _lastTaggedReleaseFinder;
        private readonly BuildNumberCalculator _sut;
        private readonly IGitHelper _gitHelper;
        private readonly IBuildServer _buildServer;

        public BuildNumberCalculatorTests()
        {
            var gitRepo = Substitute.For<IRepository>();
            _semanticVersion = Substitute.For<ISemanticVersionCalculator>();
            _lastTaggedReleaseFinder = Substitute.For<ILastTaggedReleaseFinder>();
            _gitHelper = Substitute.For<IGitHelper>();
            _buildServer = Substitute.For<IBuildServer>();
            var log = Substitute.For<ILog>();
            _sut = new BuildNumberCalculator(_semanticVersion, _lastTaggedReleaseFinder, _gitHelper, gitRepo, _buildServer, log);
        }

        [Fact]
        public void ReturnsSameSemanticVersionAsNextVersionCalculator()
        {
            _lastTaggedReleaseFinder.GetVersion().Returns(new VersionTaggedCommit(null, null));
            var semver = new SemanticVersion(0, 1, 2);
            _semanticVersion.NextVersion().Returns(semver);

            var buildNumber = _sut.GetBuildNumber();

            Assert.Equal(semver, buildNumber);
        }

        [Fact]
        public void AppendsBuildOfCommitsSinceLastReleaseAsBuildMetaData()
        {
            _lastTaggedReleaseFinder.GetVersion().Returns(new VersionTaggedCommit(null, null));
            _gitHelper.NumberOfCommitsOnBranchSinceCommit(Arg.Any<Branch>(), Arg.Any<Commit>()).Returns(5);
            var semver = new SemanticVersion(0, 1, 2);
            _semanticVersion.NextVersion().Returns(semver);

            var buildNumber = _sut.GetBuildNumber();

            Assert.Equal("0.1.2+005", buildNumber.ToString());
        }

        [Fact]
        public void PullRequestsAreTaggedWithPreReleaseSemverTag()
        {
            _lastTaggedReleaseFinder.GetVersion().Returns(new VersionTaggedCommit(null, null));
            _buildServer.IsBuildingAPullRequest(Arg.Any<IRepository>()).Returns(true);
            _buildServer.CurrentPullRequestNo(Arg.Any<Branch>()).Returns(3);
            _gitHelper.NumberOfCommitsOnBranchSinceCommit(Arg.Any<Branch>(), Arg.Any<Commit>()).Returns(5);
            var semver = new SemanticVersion(0, 1, 2);
            _semanticVersion.NextVersion().Returns(semver);

            var buildNumber = _sut.GetBuildNumber();

            Assert.Equal("0.1.2-PullRequest3+005", buildNumber.ToString());
        }
    }
}