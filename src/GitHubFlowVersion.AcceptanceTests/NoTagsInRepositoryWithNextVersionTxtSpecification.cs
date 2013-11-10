using System;
using GitHubFlowVersion.AcceptanceTests.Helpers;
using Xunit;

namespace GitHubFlowVersion.AcceptanceTests
{
    public class NoTagsInRepositoryWithNextVersionTxtSpecification : RepositorySpecification
    {
        private const string ExpectedNextVersion = "0.2.0";
        private ExecutionResults _result;

        public void GivenARepositoryWithCommitsButNoTags()
        {
            Repository.MakeACommit();
            Repository.MakeACommit();
            Repository.MakeACommit();
        }

        public void AndGivenANextVersionTxtFile()
        {
            Repository.AddNextVersionTxtFile(ExpectedNextVersion);
        }

        public void AndGivenRunningInTeamCity()
        {
            Environment.SetEnvironmentVariable("TEAMCITY_VERSION", "8.0.4");
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
            _result.Output.ShouldContainCorrectBuildVersion(ExpectedNextVersion, 2);
        }
    }
}