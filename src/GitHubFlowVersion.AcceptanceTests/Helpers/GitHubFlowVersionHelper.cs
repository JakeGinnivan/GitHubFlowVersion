using System.Diagnostics;
using System.IO;

namespace GitHubFlowVersion.AcceptanceTests.Helpers
{
    public static class GitHubFlowVersionHelper
    {
        public static Process ExecuteIn(string workingDirectory)
        {
            var gitHubFlowVersion = Path.Combine(PathHelper.GetCurrentDirectory(), "GitHubFlowVersion.exe");
            var startInfo = new ProcessStartInfo(gitHubFlowVersion)
            {
                WorkingDirectory = workingDirectory,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                ErrorDialog = false,
                UseShellExecute = false,
                Arguments = string.Format("-w \"{0}\"", workingDirectory),
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            var process = ProcessHelper.Start(startInfo);
            process.WaitForExit();

            return process;
        }
    }
}
