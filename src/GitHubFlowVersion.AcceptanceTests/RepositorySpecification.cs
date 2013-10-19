using System;
using System.IO;
using GitHubFlowVersion.AcceptanceTests.Helpers;
using LibGit2Sharp;
using Xunit;
using TestStack.BDDfy;

namespace GitHubFlowVersion.AcceptanceTests
{
    public abstract class RepositorySpecification : IDisposable
    {
        protected readonly string RepositoryPath;
        protected readonly Repository Repository;

        protected RepositorySpecification()
        {
            RepositoryPath = PathHelper.GetTempPath();
            Repository.Init(RepositoryPath);
            Console.WriteLine("Created git repository at {0}", RepositoryPath);

            Repository = new Repository(RepositoryPath);
            Repository.Config.Set("user.name", "Test");
            Repository.Config.Set("user.email", "test@email.com");
        }

        [Fact]
        public void RunSpecification()
        {
            this.BDDfy();
        }

        public void Dispose()
        {
            Repository.Dispose();
            try
            {
                Directory.Delete(RepositoryPath, true);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to clean up repository path at {0}. Received exception: {1}", RepositoryPath, e);
            }
        }
    }
}
