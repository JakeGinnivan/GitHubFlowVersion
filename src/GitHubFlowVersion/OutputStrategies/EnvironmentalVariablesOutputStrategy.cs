using System;
using System.Collections.Generic;

namespace GitHubFlowVersion.OutputStrategies
{
    public class EnvironmentalVariablesOutputStrategy : IOutputStrategy
    {
        public void Write(GitHubFlowArguments gitHubFlowConfiguration, IEnumerable<Tuple<string, string>> variables, SemanticVersion nextBuildNumber)
        {
            foreach (var variable in variables)
            {
                Environment.SetEnvironmentVariable(variable.Item1, variable.Item2);
            }
        }
    }
}