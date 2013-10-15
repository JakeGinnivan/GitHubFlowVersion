using System.Linq;
using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public class GitHelper : IGitHelper
    {
        public int NumberOfCommitsOnBranchSinceCommit(Branch branch, Commit commit)
        {
            var olderThan = branch.Tip.Committer.When;
            return branch.Commits
                .TakeWhile(x => x != commit)
                .Count();
        }

        public bool IsPullRequest(Branch branch)
        {
            return branch.CanonicalName.Contains("/pull/") || TeamCity.IsBuildingAPullRequest();
        }
    }
}