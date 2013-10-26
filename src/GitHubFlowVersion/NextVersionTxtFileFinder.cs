using System;
using System.IO;

namespace GitHubFlowVersion
{
    public class NextVersionTxtFileFinder : INextVersionTxtFileFinder
    {
        private readonly string _workingDirectory;

        public NextVersionTxtFileFinder(string workingDirectory)
        {
            _workingDirectory = workingDirectory;
        }

        public SemanticVersion GetNextVersion(SemanticVersion taggedVersion)
        {
            var filePath = Path.Combine(_workingDirectory, "NextVersion.txt");
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, taggedVersion.ToString());
            }
            var version = File.ReadAllText(filePath);

            SemanticVersion semanticVersion;
            if (!SemanticVersionParser.TryParse(version, out semanticVersion))
                throw new ArgumentException("Make sure you have a valid semantic version in NextVersion.txt");
                
            return semanticVersion;
        }
    }
}