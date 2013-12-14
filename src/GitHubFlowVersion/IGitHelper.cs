using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public interface IGitHelper
    {
        int NumberOfCommitsOnBranchSinceCommit(Branch branch, Commit commit);
        void EnsurePullBranchShareACommonAncestorWithDevelop(IRepository repository, Branch pullBranch);
        Branch GetBranch(IRepository repository, string name);
        void NormalizeGitRepository(IRepository repository);
    }
}