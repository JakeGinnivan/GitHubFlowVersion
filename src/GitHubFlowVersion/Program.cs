using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Args.Help;
using Args.Help.Formatters;
using GitHubFlowVersion.BuildServers;
using GitHubFlowVersion.OutputStrategies;
using LibGit2Sharp;

namespace GitHubFlowVersion
{
    public class Program
    {
        private const string MsBuild = @"c:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe";

        public static int Main(string[] args)
        {
            var modelBindingDefinition = Args.Configuration.Configure<GitHubFlowArguments>();
            var arguments = modelBindingDefinition.CreateAndBind(args);

            if (args.Any(a => a == "/?" || a == "?" || a.Equals("/help", StringComparison.InvariantCultureIgnoreCase)))
            {
                var help = new HelpProvider().GenerateModelHelp(modelBindingDefinition);
                var f = new ConsoleHelpFormatter();
                f.WriteHelp(help, Console.Out);

                return 0;
            }

            var context = CreateContext(arguments);

            try
            {
                Run(context);
                using (var assemblyInfoUpdate = new AssemblyInfoUpdate(new FileSystem(), context))
                {
                    var execRun = RunExecCommandIfNeeded(context);
                    var msbuildRun = RunMsBuildIfNeeded(context);
                    if (!(execRun || msbuildRun))
                    {
                        assemblyInfoUpdate.DoNotRestoreAssemblyInfo();
                        if (!context.CurrentBuildServer.IsRunningInBuildAgent())
                        {
                            Console.WriteLine("WARNING: Not running in build server and /ProjectFile or /Exec arguments not passed");
                            Console.WriteLine();
                            Console.WriteLine("Run GitHubFlowVersion.exe /? for help");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return 1;
            }

            return 0;
        }

        private static GitHubFlowVersionContext CreateContext(GitHubFlowArguments arguments)
        {
            var context = new GitHubFlowVersionContext
            {
                Arguments = arguments,
                WorkingDirectory =
                    arguments.WorkingDirectory ?? 
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            };
            var fallbackStrategy = new LocalBuild();
            var buildServers = new IBuildServer[] {new TeamCity(context)};
            context.CurrentBuildServer = buildServers.FirstOrDefault(s => s.IsRunningInBuildAgent()) ?? fallbackStrategy;

            context.GitDirectory = GitDirFinder.TreeWalkForGitDir(context.WorkingDirectory);
            if (string.IsNullOrEmpty(context.GitDirectory))
            {
                if (context.CurrentBuildServer.IsRunningInBuildAgent()) //fail the build if we're on a TC build agent
                {
                    // This exception might have to change when more build servers are added
                    throw new Exception("Failed to find .git directory on agent. " +
                                        "Please make sure agent checkout mode is enabled for you VCS roots - " +
                                        "http://confluence.jetbrains.com/display/TCD8/VCS+Checkout+Mode");
                }

                throw new Exception("Failed to find .git directory.");
            }

            Console.WriteLine("Git directory found at {0}", context.GitDirectory);
            context.RepositoryRoot = Directory.GetParent(context.GitDirectory).FullName;

            return context;
        }

        private static bool RunMsBuildIfNeeded(GitHubFlowVersionContext context)
        {
            if (string.IsNullOrEmpty(context.Arguments.ProjectFile)) return false;

            var targetsArg = context.Arguments.Targets == null ? null : " /target:" + context.Arguments.Targets;
            Console.WriteLine("Launching {0} \"{1}\"{2}", MsBuild, context.Arguments.ProjectFile, targetsArg);
            var results = ProcessHelper.Run(
                Console.WriteLine, Console.Error.WriteLine,
                null, MsBuild, string.Format("\"{0}\"{1}", context.Arguments.ProjectFile, targetsArg), context.RepositoryRoot);

            if (results != 0)
                throw new Exception("MsBuild execution failed, non-zero return code");

            return true;
        }

        private static bool RunExecCommandIfNeeded(GitHubFlowVersionContext context)
        {
            if (string.IsNullOrEmpty(context.Arguments.Exec)) return false;

            Console.WriteLine("Launching {0} {1}", context.Arguments.Exec, context.Arguments.ExecArgs);
            var results = ProcessHelper.Run(
                Console.WriteLine, Console.Error.WriteLine,
                null, context.Arguments.Exec, context.Arguments.ExecArgs, context.RepositoryRoot);
            if (results != 0)
                throw new Exception("MsBuild execution failed, non-zero return code");
            return true;
        }

        private static void Run(GitHubFlowVersionContext context)
        {
            var gitHelper = new GitHelper();
            var gitRepo = new Repository(context.GitDirectory);
            var lastTaggedReleaseFinder = new LastTaggedReleaseFinder(gitRepo, gitHelper, context.WorkingDirectory);
            var nextSemverCalculator = new NextSemverCalculator(new NextVersionTxtFileFinder(context.RepositoryRoot),
                lastTaggedReleaseFinder);
            var buildNumberCalculator = new BuildNumberCalculator(nextSemverCalculator, lastTaggedReleaseFinder, gitHelper,
                gitRepo, context.CurrentBuildServer);

            context.NextBuildNumber = buildNumberCalculator.GetBuildNumber();

            var variableProvider = new VariableProvider();
            context.Variables =  variableProvider.GetVariables(context.NextBuildNumber);
            WriteResults(context);
        }

        private static void WriteResults(GitHubFlowVersionContext context)
        {
            var outputStrategies = new IOutputStrategy[]
            {
                new BuildServerOutputStrategy(context.CurrentBuildServer),
                new JsonFileOutputStrategy(),
                new EnvironmentalVariablesOutputStrategy()
            };
            foreach (var outputStrategy in outputStrategies)
            {
                outputStrategy.Write(context);
            }
        }
    }
}