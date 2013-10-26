using System;
using GitHubFlowVersion.AcceptanceTests.Helpers;
using LibGit2Sharp;
using Xunit;

namespace GitHubFlowVersion.AcceptanceTests
{
    public abstract class PullRequestsSpecification : RepositorySpecification
    {
        private ExecutionResults _result;
        private const string TaggedVersion = "1.0.3";
        protected abstract string PullRequestBranchName();

        public void GivenATagOnMaster()
        {
            Repository.MakeATaggedCommit(TaggedVersion);
        }

        public void AndGivenAFeatureBranchWithTwoCommits()
        {
            var branch = Repository.CreateBranch("FeatureBranch");
            Repository.Checkout(branch);
            Repository.MakeCommits(2);
        }

        public void AndGivenRunningInTeamCity()
        {
            Environment.SetEnvironmentVariable("TEAMCITY_VERSION", "8.0.4");
        }

        public void AndGivenAnEnvironmentalVariableWithPullRequestRefBranch()
        {
            Environment.SetEnvironmentVariable("teamcity.build.vcs.branch.myvcsrootname", PullRequestBranchName());
        }

        public void WhenGitHubFlowVersionIsExecuted()
        {
            _result = GitHubFlowVersionHelper.ExecuteIn(RepositoryPath);
        }

        public void ThenItShouldExitWithoutError()
        {
            _result.AssertExitedSuccessfully();
        }

        public void AndTheCorrectVersionShouldBeOutput()
        {
            Assert.Contains("1.0.4-PullRequest5", _result.Output);
        }

        protected override void Cleanup()
        {
            Environment.SetEnvironmentVariable("teamcity.build.vcs.branch.myvcsrootname", null);
        }

        public class StashPullRequests : PullRequestsSpecification
        {
            protected override string PullRequestBranchName()
            {
                return "refs/pull-requests/5/merge-clean";
            }
        }

        public class GitHubPullRequests : PullRequestsSpecification
        {
            protected override string PullRequestBranchName()
            {
                return "refs/pull/5/merge";
            }
        }
    }
}