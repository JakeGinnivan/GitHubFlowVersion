using System.Collections.Generic;
using GitVersion.BuildServers;

namespace GitVersion
{
    public class GitVersionContext
    {
        public string GitDirectory { get; set; }
        public string WorkingDirectory { get; set; }
        public string RepositoryRoot { get; set; }
        public IBuildServer CurrentBuildServer { get; set; }
        public SemanticVersion NextBuildNumber { get; set; }
        public Dictionary<string, string> Variables { get; set; }
        public GitHubFlowArguments Arguments { get; set; }
    }
}