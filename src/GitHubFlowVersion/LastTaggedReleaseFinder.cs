using System;
using System.Linq;
using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public class LastTaggedReleaseFinder : ILastTaggedReleaseFinder
    {
        private readonly Lazy<VersionTaggedCommit> _lastTaggedRelease;

        public LastTaggedReleaseFinder(IRepository gitRepo, IGitHelper gitHelper)
        {
            _lastTaggedRelease = new Lazy<VersionTaggedCommit>(()=>GetVersion(gitRepo, gitHelper));
        }

        public VersionTaggedCommit GetVersion()
        {
            return _lastTaggedRelease.Value;
        }

        private static VersionTaggedCommit GetVersion(IRepository gitRepo, IGitHelper gitHelper)
        {
            var tags = gitRepo.Tags.Select(t =>
            {
                SemanticVersion version;
                if (SemanticVersionParser.TryParse(t.Name, out version))
                {
                    return new VersionTaggedCommit((Commit) t.Target, version);
                }
                return null;
            })
                .Where(a => a != null)
                .ToArray();
            var branch = gitHelper.GetBranch(gitRepo, "master");
            var olderThan = branch.Tip.Committer.When;
            var lastTaggedCommit =
                branch.Commits.FirstOrDefault(c => c.Committer.When <= olderThan && c != branch.Tip && tags.Any(a => a.Commit == c));

            if (lastTaggedCommit != null)
                return tags.Single(a => a.Commit.Sha == lastTaggedCommit.Sha);

            throw new Exception("Cant find last tagged version");
        }
    }
}