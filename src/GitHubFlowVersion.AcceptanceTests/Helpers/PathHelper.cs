using System.IO;
using System.Reflection;

namespace GitHubFlowVersion.AcceptanceTests.Helpers
{
    public static class PathHelper
    {
        public static string GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Replace(@"file:///", ""));
        }

        public static string GetGitTempPath()
        {
            var target = Path.Combine(Path.GetDirectoryName(Path.GetTempFileName()), "GitTemp");
            if (Directory.Exists(target))
                Directory.Delete(target, true);

            return target;
        } 
    }
}