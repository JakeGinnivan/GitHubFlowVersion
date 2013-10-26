using System;
using System.IO;
using GitHubFlowVersion.AcceptanceTests.Helpers;
using Xunit;

namespace GitHubFlowVersion.AcceptanceTests
{
    public class TagFollowedByCommitsWithNoNextVersionTxtSpecification : RepositorySpecification
    {
        private ExecutionResults _result;

        public void GivenARepositoryWithASingleTagFollowedByCommits()
        {
            Repository.MakeATaggedCommit("0.1.0");
            Repository.MakeACommit();
        }

        public void AndGivenThereIsNoNextVersionTxtFile() {}

        public void AndGivenRunningInTeamCity()
        {
            Environment.SetEnvironmentVariable("TEAMCITY_VERSION", "8.0.4");
        }

        public void WhenGitHubFlowVersionIsExecuted()
        {
            _result = GitHubFlowVersionHelper.ExecuteIn(RepositoryPath);
        }

        public void ThenNoErrorShouldOccur()
        {
            _result.AssertExitedSuccessfully();
        }

        public void AndTheCorrectVersionShouldBeOutput()
        {
            Assert.Contains("0.1.1", _result.Output);
        }

        public void AndTheNextVersionTxtFileShouldBeCreatedWithLastTag()
        {
            Assert.Equal("0.1.0", File.ReadAllText(Path.Combine(RepositoryPath, "NextVersion.txt")));
        }
    }
}
