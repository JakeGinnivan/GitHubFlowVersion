using LibGit2Sharp;

namespace GitVersion.BranchingStrategies
{
    public class BranchingStrategySelector
    {
        private readonly IGitHelper _gitHelper;

        public BranchingStrategySelector(IGitHelper gitHelper)
        {
            _gitHelper = gitHelper;
        }

        public IBranchingStrategy GetCurrentStrategy(IRepository repository)
        {
            if (_gitHelper.HasBranch(repository, "develop"))
                return new GitFlow();

            return new GitHubFlow();
        }
    }
}