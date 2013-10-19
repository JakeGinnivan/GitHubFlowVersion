using System.Diagnostics;
using GitHubFlowVersion.AcceptanceTests.Helpers;
using Xunit;

namespace GitHubFlowVersion.AcceptanceTests
{
    public class RedundantNextVersionTxtVersionSpecification : RepositorySpecification
    {
        private Process _result;
        private const string TaggedVersion = "1.0.3";
        private const int CommitsAfterTaggedVersion = 1;
        private const string NextVersionTxtVersion = "1.0.0";
        private const string ExpectedNextVersion = "1.0.4";

        public void GivenARepositoryWithAtLeastOneTagAndAtLeastOneCommitThatIsntTaggedAtHead()
        {
            Repository.MakeATaggedCommit(TaggedVersion);
            Repository.MakeCommits(CommitsAfterTaggedVersion);
        }

        public void AndGivenRepositoryHasANextVersionTxtFileWithARedundantVersionNumber()
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
            var output = _result.StandardOutput.ReadToEnd();
            output.ShouldContainCorrectBuildVersion(ExpectedNextVersion, CommitsAfterTaggedVersion);
            output.ShouldContainCorrectFileVersion(ExpectedNextVersion);
        }
    }
}
