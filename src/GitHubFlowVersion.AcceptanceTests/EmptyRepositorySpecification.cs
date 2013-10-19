using System.Diagnostics;
using System.IO;
using GitHubFlowVersion.AcceptanceTests.Helpers;
using Xunit;

namespace GitHubFlowVersion.AcceptanceTests
{
    public class EmptyRepositorySpecification : RepositorySpecification
    {
        private int _exitCode;
        
        public void GivenAnEmptyRepositoryWithMasterBranchAndGitHubFlowPresent()
        {
            Repository.MakeACommit();

            foreach (var f in Directory.GetFiles(PathHelper.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories))
            {
                var targetFileName = f.Replace(PathHelper.GetCurrentDirectory(), RepositoryPath);
                var targetDirectory = Path.GetDirectoryName(targetFileName);

                if (!Directory.Exists(targetDirectory))
                {
                    Directory.CreateDirectory(targetDirectory);
                }
                File.Copy(f, targetFileName);
            }
        }
        
        public void WhenGitHubFlowVersionIsExecuted()
        {
            var gitHubFlowVersion = Path.Combine(RepositoryPath, "GitHubFlowVersion.exe");
            var startInfo = new ProcessStartInfo(gitHubFlowVersion)
            {
                WorkingDirectory = RepositoryPath,
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
    }
}
