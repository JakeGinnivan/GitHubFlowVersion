using System;
using System.Collections.Generic;

namespace GitHubFlowVersion.OutputStrategies
{
    public class EnvironmentalVariablesOutputStrategy : IOutputStrategy
    {
        public void Write(GitHubFlowArguments gitHubFlowConfiguration, Dictionary<string, string> variables, SemanticVersion nextBuildNumber)
        {
            foreach (var variable in variables)
            {
                Environment.SetEnvironmentVariable(variable.Key, variable.Value);
            }
        }
    }
}