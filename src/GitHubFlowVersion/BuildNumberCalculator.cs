using GitVersion.BuildServers;
using GitVersion.Infrastructure;
using LibGit2Sharp;

namespace GitVersion
{
    public class BuildNumberCalculator
    {
        private readonly INextSemverCalculator _nextSemverCalculator;
        private readonly ILastTaggedReleaseFinder _lastTaggedReleaseFinder;
        private readonly IGitHelper _gitHelper;
        private readonly IRepository _gitRepo;
        private readonly IBuildServer _buildServer;
        private readonly ILog _log;

        public BuildNumberCalculator(
            INextSemverCalculator nextSemverCalculator,
            ILastTaggedReleaseFinder lastTaggedReleaseFinder,
            IGitHelper gitHelper, IRepository gitRepo, 
            IBuildServer buildServer, ILog log)
        {
            _nextSemverCalculator = nextSemverCalculator;
            _lastTaggedReleaseFinder = lastTaggedReleaseFinder;
            _gitHelper = gitHelper;
            _gitRepo = gitRepo;
            _buildServer = buildServer;
            _log = log;
        }

        public SemanticVersion GetBuildNumber()
        {
            var commitsSinceLastRelease = _gitHelper.NumberOfCommitsOnBranchSinceCommit(_gitRepo.Head, _lastTaggedReleaseFinder.GetVersion().Commit);
            var semanticVersion = _nextSemverCalculator.NextVersion();
            if (_buildServer.IsBuildingAPullRequest(_gitRepo))
            {
                _gitHelper.EnsurePullBranchShareACommonAncestorWithDevelop(_gitRepo, _gitRepo.Head);
                semanticVersion = semanticVersion.WithSuffix("PullRequest" + _buildServer.CurrentPullRequestNo(_gitRepo.Head));
            }

            var withBuildMetaData = semanticVersion.WithBuildMetaData(commitsSinceLastRelease);
            _log.WriteLine("Version number is '{0}'", withBuildMetaData);
            return withBuildMetaData;
        }
    }
}