using System.Diagnostics;
using GitHubFlowVersion.AcceptanceTests.Helpers;
using Xunit;

namespace GitHubFlowVersion.AcceptanceTests
{
    public class NoTagsInRepositorySpecification : RepositorySpecification
    {
        private ExecutionResults _result;

        public void GivenARepositoryWithCommitsButNoTags()
        {
            Repository.MakeACommit();
            Repository.MakeACommit();
        }
        
        public void WhenGitHubFlowVersionIsExecuted()
        {
            _result = GitHubFlowVersionHelper.ExecuteIn(RepositoryPath);
        }

        public void ThenANonZeroExitCodeShouldOccur()
        {
            Assert.NotEqual(0, _result.ExitCode);
        }

        public void AndAnErrorAboutNotFindingTagShouldBeShown()
        {
            Assert.Contains("Cant find last tagged version", _result.Output);
        }
    }
}
