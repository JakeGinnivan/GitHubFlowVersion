using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            var variables = GetVariables(nextBuildNumber).ToArray();
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

        static IEnumerable<Tuple<string, string>> GetVariables(SemanticVersion nextBuildNumber)
        {
            yield return Tuple.Create("GitHubFlowVersion.FullSemVer", nextBuildNumber.ToString());
            yield return Tuple.Create("GitHubFlowVersion.SemVer", nextBuildNumber.WithBuildMetaData(null).ToString());
            yield return Tuple.Create("GitHubFlowVersion.FourPartVersion", nextBuildNumber.ToVersion().ToString());
            yield return Tuple.Create("GitHubFlowVersion.Major", nextBuildNumber.Major.ToString());
            yield return Tuple.Create("GitHubFlowVersion.Minor", nextBuildNumber.Minor.ToString());
            yield return Tuple.Create("GitHubFlowVersion.Patch", nextBuildNumber.Patch.ToString());
            string numOfCommitsSinceRelease = nextBuildNumber.BuildMetaData == null ? "<unknown>" : nextBuildNumber.BuildMetaData.ToString();
            yield return Tuple.Create("GitHubFlowVersion.NumCommitsSinceRelease", numOfCommitsSinceRelease);
        }
    }
}