using System;
using System.Linq;
using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public class LastTaggedReleaseFinder : ILastTaggedReleaseFinder
    {
        private readonly IRepository _gitRepo;

        public LastTaggedReleaseFinder(IRepository gitRepo)
        {
            _gitRepo = gitRepo;
        }

        public SemanticVersion GetVersion()
        {
            var tags = _gitRepo.Tags.Select(t =>
            {
                SemanticVersion version;
                if (SemanticVersionParser.TryParse(t.Name, out version))
                {
                    return new
                    {
                        Commit = t.Target,
                        SemVer = version
                    };
                }
                return null;
            })
                .Where(a=>a!= null)
                .ToArray();
            var branch = _gitRepo.Head;
            DateTimeOffset olderThan = branch.Tip.Committer.When;
            var lastTaggedCommit = branch.Commits.FirstOrDefault(c => c.Committer.When < olderThan && tags.Any(a => a.Commit == c));

            //var commitsSinceLastRelease = currentBranch.Commits
            //                                  .SkipWhile(x => x != lastTaggedCommit)
            //                                  .TakeWhile(x => x.Committer.When >= olderThan)
            //                                  .Count();

            if (lastTaggedCommit != null)
                return tags.Single(a => a.Commit.Sha == lastTaggedCommit.Sha).SemVer;

            throw new Exception("Cant find last tagged version");
        }
    }
}