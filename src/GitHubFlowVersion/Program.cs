using System.Diagnostics;
using System.IO;
using System.Reflection;
using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public class Program
    {
        public static int  Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string gitDirectory = GitDirFinder.TreeWalkForGitDir(currentDirectory);
            if (string.IsNullOrEmpty(gitDirectory))
            {
                if (TeamCity.IsRunningInBuildAgent()) //fail the build if we're on a TC build agent
                {
                    Trace.TraceError("Failed to find .git directory on agent. Please make sure agent checkout mode is enabled for you VCS roots - http://confluence.jetbrains.com/display/TCD8/VCS+Checkout+Mode");
                    return -1;
                }

                Trace.TraceError("Failed to find .git directory.");
                return 0;
            }
            var gitHelper = new GitHelper();
            var gitRepo = new Repository(gitDirectory);
            var lastTaggedReleaseFinder = new LastTaggedReleaseFinder(gitRepo, gitHelper);
            var nextSemverCalculator = new NextSemverCalcualtor(new NextVersionTxtFileFinder(), lastTaggedReleaseFinder);
            var buildNumberCalculator = new BuildNumberCalculator(nextSemverCalculator, lastTaggedReleaseFinder, gitHelper, gitRepo);

            var nextBuildNumber = buildNumberCalculator.GetBuildNumber();
            TeamCityVersionWriter.WriteBuildNumber(nextBuildNumber);
            TeamCityVersionWriter.WriteAssemblyFileVersion(nextBuildNumber);
            return 0;
        } 
    }
}