using System;
using System.IO;
using System.Text;
using LibGit2Sharp;
using Xunit;

namespace GitHubFlowVersion.AcceptanceTests.Helpers
{
    public static class GitHubFlowVersionHelper
    {
        public static ExecutionResults ExecuteIn(string workingDirectory, string toFile = null, 
            string exec = null, string execArgs = null, string projectFile = null, string targets = null)
        {
            var gitHubFlowVersion = Path.Combine(PathHelper.GetCurrentDirectory(), "GitHubFlowVersion.exe");
            string toFileArg = toFile == null ? null : string.Format(" /ToFile \"{0}\"", toFile);
            string execArg = exec == null ? null : string.Format(" /Exec \"{0}\"", exec);
            string execArgsArg = execArgs == null ? null : string.Format(" /ExecArgs \"{0}\"", execArgs);
            string projectFileArg = projectFile == null ? null : string.Format(" /ProjectFile \"{0}\"", projectFile);
            string targetsArg = targets == null ? null : string.Format(" /Targets \"{0}\"", targets);
            var arguments = string.Format("/w \"{0}\"{1}{2}{3}{4}{5}", workingDirectory, toFileArg, execArg, execArgsArg,
                projectFileArg, targetsArg);

            var output = new StringBuilder();

            Console.WriteLine("Executing: {0} {1}", gitHubFlowVersion, arguments);
            Console.WriteLine();
            var exitCode = ProcessHelper.Run(s => output.AppendLine(s), s => output.AppendLine(s), null, gitHubFlowVersion, arguments, workingDirectory);

            Console.WriteLine("Output from GitHubFlowVersion.exe");
            Console.WriteLine("-------------------------------------------------------");
            Console.WriteLine(output.ToString());
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------------");

            return new ExecutionResults(exitCode, output.ToString());
        }

        public static void ShouldContainCorrectBuildVersion(this string output, string version, int commitsSinceTag)
        {
            Assert.Contains(string.Format("Version number is '{0}+{1:000}'", version, commitsSinceTag), output);
        }

        public static void AddNextVersionTxtFile(this IRepository repository, string version)
        {
            var nextVersionFile = Path.Combine(repository.Info.WorkingDirectory, "NextVersion.txt");
            File.WriteAllText(nextVersionFile, version);
        }
    }
}
