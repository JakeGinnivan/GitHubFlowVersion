using GitVersion.BranchingStrategies;
using GitVersion.Infrastructure;
using GitVersion.OutputStrategies;
using LibGit2Sharp;

namespace GitVersion
{
    public class GitVersionCalculator
    {
        public static void Run(GitVersionContext context, IGitHelper gitHelper, ILog log)
        {
            using (var gitRepo = new Repository(context.GitDirectory))
            {
                var lastTaggedReleaseFinder = new LastTaggedReleaseFinder(gitRepo, gitHelper, context.WorkingDirectory);
                var strategySelector = new BranchingStrategySelector(gitHelper, lastTaggedReleaseFinder);
                var branchingStrategy = strategySelector.GetCurrentStrategy(gitRepo);
                var nextSemverCalculator = branchingStrategy.GetNextSemverCalculator(context);
                var buildNumberCalculator = new BuildNumberCalculator(nextSemverCalculator, lastTaggedReleaseFinder, gitHelper,
                    gitRepo, context.CurrentBuildServer, log);

                context.NextBuildNumber = buildNumberCalculator.GetBuildNumber();

                var variableProvider = new VariableProvider();
                context.Variables = variableProvider.GetVariables(context.NextBuildNumber);
                WriteResults(context);
            }
        }

        static void WriteResults(GitVersionContext context)
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