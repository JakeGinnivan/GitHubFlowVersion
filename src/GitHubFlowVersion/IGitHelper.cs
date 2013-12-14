using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public interface IGitHelper
    {
        int NumberOfCommitsOnBranchSinceCommit(Branch branch, Commit commit);
        Branch GetBranch(IRepository repository, string name);
        void EnsurePullBranchShareACommonAncestorWithDevelop(IRepository repository, Branch pullBranch);
    }
}