using System.Diagnostics;
using GitHubFlowVersion.AcceptanceTests.Helpers;
using Xunit;

namespace GitHubFlowVersion.AcceptanceTests
{
    public class TagFollowedByCommitsWithNoNextVersionTxtSpecification : RepositorySpecification
    {
        private Process _result;

        public void GivenARepositoryWithASingleTagFollowedByCommits()
        {
            Repository.MakeATaggedCommit("0.1.0");
            Repository.MakeACommit();
        }

        public void AndGivenThereIsNoNextVersionTxtFile() {}
        
        public void WhenGitHubFlowVersionIsExecuted()
        {
            _result = GitHubFlowVersionHelper.ExecuteIn(RepositoryPath);
        }

        public void ThenANonZeroExitCodeShouldOccur()
        {
            Assert.NotEqual(0, _result.ExitCode);
        }

        public void AndTheCorrectVersionShouldBeOutput()
        {
            Assert.Contains("", _result.StandardError.ReadToEnd());
        }
    }
}
