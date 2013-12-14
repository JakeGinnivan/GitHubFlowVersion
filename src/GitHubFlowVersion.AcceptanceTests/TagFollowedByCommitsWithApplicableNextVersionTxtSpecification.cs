using System;
using GitHubFlowVersion.AcceptanceTests.Helpers;
using Xunit;
using TestStack.BDDfy;
using Xunit.Extensions;

namespace GitHubFlowVersion.AcceptanceTests
{
    public class TagFollowedByCommitsWithApplicableNextVersionTxtSpecification : RepositorySpecification
    {
        private ExecutionResults _result;
        private const string TaggedVersion = "1.0.3";
        private int _numCommitsToMake;
        private const string NextVersionTxtVersion = "1.1.0";
        private const string ExpectedNextVersion = "1.1.0";

        public void GivenARepositoryWithASingleTag()
        {
            Repository.MakeATaggedCommit(TaggedVersion);
        }

        public void AndGivenRepositoryHasAnotherXCommits()
        {
            Repository.MakeCommits(_numCommitsToMake);
        }

        public void AndGivenRepositoryHasARedundantNextVersionTxtFile()
        {
            Repository.AddNextVersionTxtFile(NextVersionTxtVersion);
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
            _result.Output.ShouldContainCorrectBuildVersion(ExpectedNextVersion, _numCommitsToMake);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public void Run(int noCommits)
        {
            _numCommitsToMake = noCommits;
            Environment.SetEnvironmentVariable("TEAMCITY_VERSION", null);
            this.BDDfy();
        }

        protected new void RunSpecification() { }
    }
}
