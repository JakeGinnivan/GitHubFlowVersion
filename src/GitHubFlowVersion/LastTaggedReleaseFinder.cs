using System;
using System.Linq;
using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public class LastTaggedReleaseFinder : ILastTaggedReleaseFinder
    {
        private readonly Lazy<VersionTaggedCommit> _lastTaggedRelease;

        public LastTaggedReleaseFinder(IRepository gitRepo)
        {
            _lastTaggedRelease = new Lazy<VersionTaggedCommit>(()=>GetVersion(gitRepo));
        }

        public VersionTaggedCommit GetVersion()
        {
            return _lastTaggedRelease.Value;
        }

        private VersionTaggedCommit GetVersion(IRepository gitRepo)
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
            var branch = gitRepo.Head;
            var olderThan = branch.Tip.Committer.When;
            var lastTaggedCommit =
                branch.Commits.FirstOrDefault(c => c.Committer.When < olderThan && tags.Any(a => a.Commit == c));

            if (lastTaggedCommit != null)
                return tags.Single(a => a.Commit.Sha == lastTaggedCommit.Sha);

            throw new Exception("Cant find last tagged version");
        }
    }
}