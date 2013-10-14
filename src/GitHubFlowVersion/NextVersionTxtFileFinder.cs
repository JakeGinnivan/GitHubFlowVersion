using System;
using System.IO;
using System.Reflection;

namespace GitHubFlowVersion
{
    public class NextVersionTxtFileFinder : INextVersionTxtFileFinder
    {
        public SemanticVersion GetNextVersion()
        {
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var repoPath = GitDirFinder.TreeWalkForGitDir(currentDirectory);

            var version = File.ReadAllText(Path.Combine(repoPath, "..\\NextVersion.txt"));

            SemanticVersion semanticVersion;
            if (!SemanticVersionParser.TryParse(version, out semanticVersion))
                throw new ArgumentException("Make sure you have a valid semantic version in NextVersion.txt");
                
            return semanticVersion;
        }
    }
}