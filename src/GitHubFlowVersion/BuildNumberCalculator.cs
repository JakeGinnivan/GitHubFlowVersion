using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public class BuildNumberCalculator
    {
        private readonly INextSemverCalcualtor _nextSemverCalculator;
        private readonly ILastTaggedReleaseFinder _lastTaggedReleaseFinder;
        private readonly IGitHelper _gitHelper;
        private readonly IRepository _gitRepo;

        public BuildNumberCalculator(
            INextSemverCalcualtor nextSemverCalculator,
            ILastTaggedReleaseFinder lastTaggedReleaseFinder,
            IGitHelper gitHelper, IRepository gitRepo)
        {
            _nextSemverCalculator = nextSemverCalculator;
            _lastTaggedReleaseFinder = lastTaggedReleaseFinder;
            _gitHelper = gitHelper;
            _gitRepo = gitRepo;
        }

        public SemanticVersion GetBuildNumber()
        {
            var currentBranch  = _gitRepo.Head;
            var commitsSinceLastRelease = _gitHelper.NumberOfCommitsOnBranchSinceCommit(currentBranch, _lastTaggedReleaseFinder.GetVersion().Commit);
            SemanticVersion semanticVersion = _nextSemverCalculator.NextVersion();
            if (_gitHelper.IsPullRequest(currentBranch))
                semanticVersion = semanticVersion.WithSuffix("PullRequest");
            return semanticVersion.WithBuildMetaData(commitsSinceLastRelease.ToString("000"));
        }
    }
}