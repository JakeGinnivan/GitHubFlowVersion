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
        private const string NextVersion = "0.1.1";
        private int _commitsToMake;

        public void GivenARepositoryWithASingleTagAndNoNextVersionFile()
        {
            Repository.MakeATaggedCommit(TaggedVersion);
        }

        public void AndGivenRepositoryHasAnotherXCommits()
        {
            for (var i = 0; i < _commitsToMake; i++)
                Repository.MakeACommit();
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
            output.ShouldContainCorrectBuildVersion(NextVersion, _commitsToMake);
            output.ShouldContainCorrectFileVersion(NextVersion);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public void Run(int noCommits)
        {
            _commitsToMake = noCommits;
            this.BDDfy();
        }

        // todo: need to figure out a better way to get XUnit to ignore this - maybe this shouldn't be in the base class?
        public override void RunSpecification() { }

    }
}
