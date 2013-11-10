using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public class LastTaggedReleaseFinder : ILastTaggedReleaseFinder
    {
        private readonly string _workingDirectory;
        private readonly Lazy<VersionTaggedCommit> _lastTaggedRelease;

        public LastTaggedReleaseFinder(IRepository gitRepo, IGitHelper gitHelper, string workingDirectory)
        {
            _workingDirectory = workingDirectory;
            _lastTaggedRelease = new Lazy<VersionTaggedCommit>(()=>GetVersion(gitRepo, gitHelper));
        }

        public VersionTaggedCommit GetVersion()
        {
            return _lastTaggedRelease.Value;
        }

        private VersionTaggedCommit GetVersion(IRepository gitRepo, IGitHelper gitHelper)
        {
            var branch = gitHelper.GetBranch(gitRepo, "master");
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

            // Create a next version txt as 0.1.0
            var filePath = Path.Combine(_workingDirectory, "NextVersion.txt");
            if (!File.Exists(filePath))
                File.WriteAllText(filePath, "0.1.0");

            var commit = branch.Commits.Last();
            return new VersionTaggedCommit(commit, new SemanticVersion(0, 0 ,0));
        }
    }
}