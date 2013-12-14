using LibGit2Sharp;

namespace GitVersion.BranchingStrategies
{
    public class BranchingStrategySelector
    {
        private readonly ILastTaggedReleaseFinder _lastTaggedReleaseFinder;
        private readonly IGitHelper _gitHelper;

        public BranchingStrategySelector(IGitHelper gitHelper, ILastTaggedReleaseFinder lastTaggedReleaseFinder)
        {
            _gitHelper = gitHelper;
            _lastTaggedReleaseFinder = lastTaggedReleaseFinder;
        }

        public IBranchingStrategy GetCurrentStrategy(IRepository repository)
        {
            if (_gitHelper.HasBranch(repository, "develop"))
                return new GitFlowStrategy();

            return new GitHubFlowStrategy(_lastTaggedReleaseFinder);
        }
    }
}