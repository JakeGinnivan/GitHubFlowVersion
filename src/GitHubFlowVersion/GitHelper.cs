using System;
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

        public void EnsurePullBranchShareACommonAncestorWithMaster(IRepository repository, Branch pullBranch)
        {
            var ancestor = repository.Commits.FindCommonAncestor(
                GetBranch(repository, "master").Tip, pullBranch.Tip);

            if (ancestor != null)
            {
                return;
            }

            throw new Exception(
                "A pull request branch is expected to branch off of 'master'. "
                + string.Format("However, branch 'master' and '{0}' do not share a common ancestor."
                , pullBranch.Name));
        }

        public Branch GetBranch(IRepository repository, string name)
        {
            var branch = repository.Branches.FirstOrDefault(b => b.Name == name);

            if (branch == null)
            {
                if (!repository.Network.Remotes.Any())
                {
                    Console.WriteLine("No remotes found");
                }
                else
                {
                    var remote = repository.Network.Remotes.First();

                    Console.WriteLine("No local branch with name {0} found, going to try on the remote {1}({2})", name, remote.Name, remote.Url);
                    try
                    {
                        repository.Network.Fetch(remote);
                    }
                    catch (LibGit2SharpException exception)
                    {
                        if (exception.Message.Contains("This transport isn't implemented"))
                        {
                            var message = string.Format("Could not fetch from '{0}' since LibGit2 does not support the transport. You have most likely cloned using SSH. If there is a remote branch named '{1}' then fetch it manually, otherwise please create a local branch named '{1}'.", remote.Url, name);
                            throw new Exception(message, exception);
                        }
                        throw;
                    }

                    branch = repository.Branches.FirstOrDefault(b => b.Name.EndsWith("/" + name));
                }
            }

            if (branch == null)
            {
                var branchNames = string.Join(";", repository.Branches);
                var message = string.Format("Could not find branch '{0}' in the repository, please create one. Existing branches:{1}", name, branchNames);
                throw new Exception(message);
            }

            return branch;
        }

        public static void NormalizeGitRepository(IRepository repository)
        {
            EnsureOnlyOneRemoteIsDefined(repository);
            CreateMissingLocalBranchesFromRemoteTrackingOnes(repository);

            if (!repository.Info.IsHeadDetached)
            {
                return;
            }

            CreateFakeBranchPointingAtThePullRequestTip(repository);
        }

        static void EnsureOnlyOneRemoteIsDefined(IRepository repo)
        {
            var howMany = repo.Network.Remotes.Count();

            if (howMany == 1)
            {
                return;
            }

            var message = string.Format("{0} remote(s) have been detected. When being run on a TeamCity agent, the Git repository is expected to bear one (and no more than one) remote.", howMany);
            throw new Exception(message);
        }

        static void CreateMissingLocalBranchesFromRemoteTrackingOnes(IRepository repo)
        {
            var remoteName = repo.Network.Remotes.Single().Name;
            var prefix = string.Format("refs/remotes/{0}/", remoteName);

            foreach (var remoteTrackingReference in repo.Refs.FromGlob(prefix + "*"))
            {
                var localCanonicalName = "refs/heads/" + remoteTrackingReference.CanonicalName.Substring(prefix.Length);
                if (repo.Refs.Any(x => x.CanonicalName == localCanonicalName))
                {
                    Console.WriteLine("Skipping local branch creation since it already exists '{0}'.", remoteTrackingReference.CanonicalName);
                    continue;
                }
                Console.WriteLine("Creating local branch from remote tracking '{0}'.", remoteTrackingReference.CanonicalName);

                var symbolicReference = remoteTrackingReference as SymbolicReference;
                var targetId = symbolicReference == null
                    ? new ObjectId(remoteTrackingReference.TargetIdentifier)
                    : new ObjectId(symbolicReference.ResolveToDirectReference().TargetIdentifier);
                repo.Refs.Add(localCanonicalName, targetId, true);
            }
        }

        static void CreateFakeBranchPointingAtThePullRequestTip(IRepository repo)
        {
            var remote = repo.Network.Remotes.Single();
            var remoteTips = repo.Network.ListReferences(remote);

            var headTipSha = repo.Head.Tip.Sha;

            var refs = remoteTips.Where(r => r.TargetIdentifier == headTipSha).ToList();

            if (refs.Count == 0)
            {
                var message = string.Format("Couldn't find any remote tips from remote '{0}' pointing at the commit '{1}'.", remote.Url, headTipSha);
                throw new Exception(message);
            }

            if (refs.Count > 1)
            {
                var names = string.Join(", ", refs.Select(r => r.CanonicalName));
                var message = string.Format("Found more than one remote tip from remote '{0}' pointing at the commit '{1}'. Unable to determine which one to use ({2}).", remote.Url, headTipSha, names);
                throw new Exception(message);
            }

            var canonicalName = refs[0].CanonicalName;
            if (!canonicalName.StartsWith("refs/pull/") && !canonicalName.StartsWith("refs/pull-requests/"))
            {
                var message = string.Format("Remote tip '{0}' from remote '{1}' doesn't look like a valid pull request.", canonicalName, remote.Url);
                throw new Exception(message);
            }

            var fakeBranchName = canonicalName
                .Replace("refs/pull/", "refs/heads/pull/")
                .Replace("refs/pull-requests/", "refs/heads/pull-requests/");
            repo.Refs.Add(fakeBranchName, new ObjectId(headTipSha));

            Console.WriteLine("Checking out {0}", fakeBranchName);
            repo.Checkout(fakeBranchName);
        }
    }
}