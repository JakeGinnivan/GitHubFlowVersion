using System;
using System.IO;
using LibGit2Sharp;

namespace GitHubFlowVersion.AcceptanceTests.Helpers
{
    public static class GitHelper
    {
        public static void MakeACommit(this IRepository repository)
        {
            var randomFile = Path.Combine(repository.Info.WorkingDirectory, Guid.NewGuid().ToString());
            File.WriteAllText(randomFile, string.Empty);
            repository.Index.Stage(randomFile);
            repository.Commit("Test Commit", new Signature("Test User", "test@email.com", DateTimeOffset.UtcNow));
        }
    }
}
