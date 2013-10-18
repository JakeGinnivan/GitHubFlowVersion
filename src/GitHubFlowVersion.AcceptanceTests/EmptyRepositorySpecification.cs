using System;
using System.Diagnostics;
using System.IO;
using GitHubFlowVersion.AcceptanceTests.Helpers;
using LibGit2Sharp;
using TestStack.BDDfy;
using Xunit;

namespace GitHubFlowVersion.AcceptanceTests
{
    public class EmptyRepositorySpecification
    {
        private Repository _repository;
        private string _repositoryPath;
        private int _exitCode;
        
        public void GivenAnEmptyRepositoryWithMasterBranchAndGitHubFlowPresent()
        {
            _repositoryPath = PathHelper.GetGitTempPath();
            Repository.Init(_repositoryPath);
            _repository = new Repository(_repositoryPath);
            var testFilePath = Path.Combine(_repositoryPath, "test.txt");
            File.WriteAllText(testFilePath, string.Empty);
            _repository.Index.Stage(testFilePath);
            _repository.Commit("Test Commit", new Signature("Test User", "test@email.com", DateTimeOffset.UtcNow));
            _repository.Dispose();

            foreach (var f in Directory.GetFiles(PathHelper.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories))
            {
                var targetFileName = f.Replace(PathHelper.GetCurrentDirectory(), _repositoryPath);
                var targetDirectory = Path.GetDirectoryName(targetFileName);

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                File.Copy(f, targetFileName);
            }

            Console.WriteLine("Created git repository at {0}", _repositoryPath);
        }

        public void WhenGitHubFlowVersionIsExecuted()
        {
            var gitHubFlowVersion = Path.Combine(_repositoryPath, "GitHubFlowVersion.exe");
            var startInfo = new ProcessStartInfo(gitHubFlowVersion)
            {
                WorkingDirectory = _repositoryPath,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                ErrorDialog = false,
                UseShellExecute = false
            };

            var process = ProcessHelper.Start(startInfo);
            process.WaitForExit();

            _exitCode = process.ExitCode;
        }

        public void ThenANonZeroExitCodeShouldOccur()
        {
            Assert.NotEqual(0, _exitCode);
        }

        [Fact]
        public void Run()
        {
            this.BDDfy();
        }
    }
}
