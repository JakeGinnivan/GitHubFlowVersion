using System.ComponentModel;

namespace GitHubFlowVersion
{
    public class GitHubFlowArguments
    {
        [Description("The directory of the Git repository to determine the version for; " +
                     "if unspecified it will search parent directories recursively until finding a Git repository.")]
        public string WorkingDirectory { get; set; }

        [Description("A filename to write all the GitHubFlowVersion variables into (json format)")]
        public string ToFile { get; set; }

        [Description("Command line to execute, GitHubFlowVersion variables will be available as environmental variables")]
        public string Exec { get; set; }

        [Description("Arguments to pass to the executable specifiec with exec")]
        public string ExecArgs { get; set; }

        [Description("MSBuild project file to execute, GitHubFlowVersion variables will be available as MSBuild variables")]
        public string ProjectFile { get; set; }

        [Description("MSBuild targets to execute")]
        public string Targets { get; set; }

        [Description("Flag which will update all assembly info files")]
        public bool UpdateAssemblyInfo { get; set; }
    }
}