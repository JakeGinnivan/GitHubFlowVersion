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
            var branch = gitHelper.GetBranch(gitRepo, "master");
            VersionTaggedCommit versionTaggedCommit;
            try
            {
                versionTaggedCommit = GetVersionTaggedCommit(gitRepo, branch);
            }
            catch (Exception)
            {
                // If we cannot find the tagged commit, then try to fetch tags from the remote before giving up
                var remote = gitRepo.Network.Remotes.FirstOrDefault();
                if (remote == null)
                    throw;
                gitRepo.Network.Fetch(remote, TagFetchMode.All);
                versionTaggedCommit = GetVersionTaggedCommit(gitRepo, branch);
            }
            return versionTaggedCommit;
        }

        private static VersionTaggedCommit GetVersionTaggedCommit(IRepository gitRepo, Branch branch)
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
            var olderThan = branch.Tip.Committer.When;
            var lastTaggedCommit =
                branch.Commits.FirstOrDefault(c => c.Committer.When <= olderThan && tags.Any(a => a.Commit == c));

            if (lastTaggedCommit != null)
                return tags.Single(a => a.Commit.Sha == lastTaggedCommit.Sha);

            throw new Exception("Cant find last tagged version");
        }
    }
}