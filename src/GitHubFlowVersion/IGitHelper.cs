using LibGit2Sharp;

namespace GitVersion
{
    public interface IGitHelper
    {
        int NumberOfCommitsOnBranchSinceCommit(Branch branch, Commit commit);
        void EnsurePullBranchShareACommonAncestorWithDevelop(IRepository repository, Branch pullBranch);
        Branch GetBranch(IRepository repository, string name);
        void NormalizeGitRepository(IRepository repository);
        bool HasBranch(IRepository repository, string branchName);
    }
}