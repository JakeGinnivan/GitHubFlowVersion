using System.Diagnostics;
using System.IO;
using Xunit;

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

        public static void ShouldContainCorrectBuildVersion(this string output, string version, int commitsSinceTag)
        {
            Assert.Contains(string.Format("##teamcity[buildNumber '{0}+{1:000}']", version, commitsSinceTag), output);
        }

        public static void ShouldContainCorrectFileVersion(this string output, string version)
        {
            Assert.Contains(string.Format("##teamcity[setParameter name='GitHubFlowVersion.FileVersion' value='{0}']", version), output);
        }
    }
}
