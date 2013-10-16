using System.IO;
using System.Reflection;
using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var gitRepo = new Repository(GitDirFinder.TreeWalkForGitDir(currentDirectory));
            var lastTaggedReleaseFinder = new LastTaggedReleaseFinder(gitRepo);
            var nextSemverCalculator = new NextSemverCalcualtor(new NextVersionTxtFileFinder(), lastTaggedReleaseFinder);
            var buildNumberCalculator = new BuildNumberCalculator(nextSemverCalculator, lastTaggedReleaseFinder, new GitHelper(), gitRepo);

            var nextBuildNumber = buildNumberCalculator.GetBuildNumber();
            TeamCityVersionWriter.WriteBuildNumber(nextBuildNumber);
            TeamCityVersionWriter.WriteAssemblyFileVersion(nextBuildNumber);
        } 
    }
}