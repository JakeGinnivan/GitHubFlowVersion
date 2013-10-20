using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandLine;
using GitHubFlowVersion.OutputStrategies;
using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public class Program
    {
        public static int  Main(string[] args)
        {
            var arguments = new GitHubFlowArguments();
            Parser.Default.ParseArgumentsStrict(args, arguments);

            Trace.Listeners.Add(new ConsoleTraceListener());

            var workingDirectory = arguments.WorkingDirectory ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var gitDirectory = GitDirFinder.TreeWalkForGitDir(workingDirectory);
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
            var nextSemverCalculator = new NextSemverCalculator(new NextVersionTxtFileFinder(workingDirectory), lastTaggedReleaseFinder);
            var buildNumberCalculator = new BuildNumberCalculator(nextSemverCalculator, lastTaggedReleaseFinder, gitHelper, gitRepo);

            var nextBuildNumber = buildNumberCalculator.GetBuildNumber();
            var variableProvider = new VariableProvider();
            var variables = variableProvider.GetVariables(nextBuildNumber);
            var outputStrategies = new IOutputStrategy[]
            {
                new TeamCityOutputStrategy(),
                new JsonFileOutputStrategy(),
                new EnvironmentalVariablesOutputStrategy()
            };
            foreach (var outputStrategy in outputStrategies)
            {
                outputStrategy.Write(arguments, variables, nextBuildNumber);
            }
            
            return 0;
        }
    }
}