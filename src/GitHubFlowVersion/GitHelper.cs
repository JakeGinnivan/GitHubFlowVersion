﻿using System;
using System.Diagnostics;
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

        public bool IsPullRequest(Branch branch)
        {
            return branch.CanonicalName.Contains("/pull/") || TeamCity.IsBuildingAPullRequest();
        }

        public Branch GetBranch(IRepository repository, string name)
        {
            var branch = repository.Branches.FirstOrDefault(b => b.Name == name);

            if (branch == null)
            {

                if (!repository.Network.Remotes.Any())
                {
                    Trace.Write("No remotes found");
                }
                else
                {
                    var remote = repository.Network.Remotes.First();

                    Trace.Write(string.Format("No local branch with name {0} found, going to try on the remote {1}({2})", name, remote.Name, remote.Url));
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
    }
}