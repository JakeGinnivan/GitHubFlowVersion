using System.Diagnostics;
using GitHubFlowVersion.AcceptanceTests.Helpers;
using Xunit;
using TestStack.BDDfy;
using Xunit.Extensions;

namespace GitHubFlowVersion.AcceptanceTests
{
    public class SingleTagFollowedByCommitsWithNoVersionTxtFileSpecification : RepositorySpecification
    {
        private Process _result;
        private const string TaggedVersion = "0.1.0";
        private const string ExpectedNextVersion = "0.1.1";
        private int _numCommitsToMake;

        public void GivenARepositoryWithASingleTagAndNoNextVersionFile()
        {
            Repository.MakeATaggedCommit(TaggedVersion);
        }

        public void AndGivenRepositoryHasAnotherXCommits()
        {
            Repository.MakeCommits(_numCommitsToMake);
        }
        
        public void WhenGitHubFlowVersionIsExecuted()
        {
            _result = GitHubFlowVersionHelper.ExecuteIn(RepositoryPath);
        }

        public void ThenAZeroExitCodeShouldOccur()
        {
            Assert.Equal(0, _result.ExitCode);
        }

        public void AndTheCorrectVersionShouldBeOutput()
        {
            var output = _result.StandardOutput.ReadToEnd();
            output.ShouldContainCorrectBuildVersion(ExpectedNextVersion, _numCommitsToMake);
            output.ShouldContainCorrectFileVersion(ExpectedNextVersion);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public void Run(int noCommits)
        {
            _numCommitsToMake = noCommits;
            this.BDDfy();
        }

        // todo: need to figure out a better way to get XUnit to ignore this - maybe this shouldn't be in the base class?
        public override void RunSpecification() { }

    }
}
