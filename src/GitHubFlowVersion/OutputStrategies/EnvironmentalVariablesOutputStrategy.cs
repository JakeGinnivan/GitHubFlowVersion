using System;

namespace GitHubFlowVersion.OutputStrategies
{
    public class EnvironmentalVariablesOutputStrategy : IOutputStrategy
    {
        public void Write(GitHubFlowVersionContext context)
        {
            foreach (var variable in context.Variables)
            {
                Environment.SetEnvironmentVariable(variable.Key, variable.Value);
            }
        }
    }
}