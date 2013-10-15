using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public interface IGitHelper
    {
        int NumberOfCommitsOnBranchSinceCommit(Branch branch, Commit commit);
        bool IsPullRequest(Branch branch);
    }
}