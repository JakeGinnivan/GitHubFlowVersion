using System;
using System.IO;
using System.Reflection;
using GitHubFlowVersion.OutputStrategies;
using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public class Program
    {
        private const string MsBuild = @"c:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe";

        public static int Main(string[] args)
        {
            var arguments = Args.Configuration.Configure<GitHubFlowArguments>().CreateAndBind(args);

            var workingDirectory = arguments.WorkingDirectory ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                Run(arguments, workingDirectory);
                RunExecCommandIfNeeded(arguments, workingDirectory);
                RunMsBuildIfNeeded(arguments, workingDirectory);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }

            return 0;
        }

        private static void RunMsBuildIfNeeded(GitHubFlowArguments arguments, string workingDirectory)
        {
            if (!string.IsNullOrEmpty(arguments.ProjectFile))
            {
                var targetsArg = arguments.Targets == null ? null : " /target:" + arguments.Targets;
                Console.WriteLine("Launching {0} {1}{2}", MsBuild, arguments.ProjectFile, targetsArg);
                ProcessHelper.Run(
                    Console.WriteLine, Console.Error.WriteLine,
                    null, MsBuild, arguments.ProjectFile + targetsArg, workingDirectory);
            }
        }

        private static void RunExecCommandIfNeeded(GitHubFlowArguments arguments, string workingDirectory)
        {
            if (!string.IsNullOrEmpty(arguments.Exec))
            {
                Console.WriteLine("Launching {0} {1}", arguments.Exec, arguments.ExecArgs);
                ProcessHelper.Run(
                    Console.WriteLine, Console.Error.WriteLine,
                    null, arguments.Exec, arguments.ExecArgs, workingDirectory);
            }
        }

        private static void Run(GitHubFlowArguments arguments, string workingDirectory)
        {
            var gitDirectory = GitDirFinder.TreeWalkForGitDir(workingDirectory);
            if (string.IsNullOrEmpty(gitDirectory))
            {
                if (TeamCity.IsRunningInBuildAgent()) //fail the build if we're on a TC build agent
                {
                    throw new Exception("Failed to find .git directory on agent. " +
                                        "Please make sure agent checkout mode is enabled for you VCS roots - " +
                                        "http://confluence.jetbrains.com/display/TCD8/VCS+Checkout+Mode");
                }

                throw new Exception("Failed to find .git directory.");
            }

            Console.WriteLine("Git directory found at {0}", gitDirectory);
            var repositoryRoot = Directory.GetParent(gitDirectory).FullName;

            var gitHelper = new GitHelper();
            var gitRepo = new Repository(gitDirectory);
            var lastTaggedReleaseFinder = new LastTaggedReleaseFinder(gitRepo, gitHelper);
            var nextSemverCalculator = new NextSemverCalculator(new NextVersionTxtFileFinder(repositoryRoot),
                lastTaggedReleaseFinder);
            var buildNumberCalculator = new BuildNumberCalculator(nextSemverCalculator, lastTaggedReleaseFinder, gitHelper,
                gitRepo);

            var nextBuildNumber = buildNumberCalculator.GetBuildNumber();
            WriteResults(arguments, nextBuildNumber);
        }

        private static void WriteResults(GitHubFlowArguments arguments, SemanticVersion nextBuildNumber)
        {
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
        }
    }
}