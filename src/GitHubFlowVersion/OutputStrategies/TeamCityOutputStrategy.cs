using System.Collections.Generic;

namespace GitHubFlowVersion.OutputStrategies
{
    public class TeamCityOutputStrategy : IOutputStrategy
    {
        public void Write(GitHubFlowArguments gitHubFlowConfiguration, Dictionary<string, string> variables, SemanticVersion nextBuildNumber)
        {
            if (TeamCity.IsRunningInBuildAgent())
            {
                TeamCityVersionWriter.WriteBuildNumber(nextBuildNumber);
                foreach (var variable in variables)
                {
                    TeamCityVersionWriter.WriteParameter(variable.Key.Replace('_', '.'), variable.Value);
                }
            }
        }
    }
}