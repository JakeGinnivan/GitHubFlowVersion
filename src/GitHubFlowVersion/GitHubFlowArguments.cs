using CommandLine;

namespace GitHubFlowVersion
{
    public class GitHubFlowArguments
    {
        [Option('w', "WorkingDirectory", DefaultValue = null, Required = false,
            HelpText =
                "The directory of the Git repository to determine the version for; if unspecified it will search parent directories recursively until finding a Git repository."
            )]
        public string WorkingDirectory { get; set; }

        [Option('f', "ToFile",
            HelpText = "A filename to write all the GitHubFlowVersion variables into (json format)")]
        public string ToFile { get; set; }

        [Option('e', "Exec",
            HelpText = "Command line to execute, GitHubFlowVersion variables will be available as environmental variables")]
        public string Execute { get; set; }

        [Option('p', "Project",
            HelpText = "MSBuild project file to execute, GitHubFlowVersion variables will be available as MSBuild variables")]
        public string Project { get; set; }

        [Option('t', "ProjectTargets",
            HelpText = "MSBuild targets to execute")]
        public string ProjectTargets { get; set; }
    }
}