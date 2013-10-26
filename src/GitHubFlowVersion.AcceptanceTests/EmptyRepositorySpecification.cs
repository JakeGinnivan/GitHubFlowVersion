using GitHubFlowVersion.AcceptanceTests.Helpers;
using Xunit;

namespace GitHubFlowVersion.AcceptanceTests
{
    public class EmptyRepositorySpecification : RepositorySpecification
    {
        private ExecutionResults _result;

        public void GivenAnEmptyRepository() {}
        
        public void WhenGitHubFlowVersionIsExecuted()
        {
            _result = GitHubFlowVersionHelper.ExecuteIn(RepositoryPath);
        }

        public void ThenANonZeroExitCodeShouldOccur()
        {
            Assert.NotEqual(0, _result.ExitCode);
        }

        public void AndAnErrorAboutNotFindingMasterShouldBeShown()
        {
            Assert.Contains("Could not find branch 'master' in the repository", _result.Output);
        }
    }
}
