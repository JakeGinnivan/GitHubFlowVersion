using System;
using System.Collections.Generic;

namespace GitHubFlowVersion.OutputStrategies
{
    public class TeamCityOutputStrategy : IOutputStrategy
    {
        public void Write(GitHubFlowArguments gitHubFlowConfiguration, IEnumerable<Tuple<string, string>> variables, SemanticVersion nextBuildNumber)
        {
            if (TeamCity.IsRunningInBuildAgent())
            {
                TeamCityVersionWriter.WriteBuildNumber(nextBuildNumber);
                foreach (var variable in variables)
                {
                    TeamCityVersionWriter.WriteParameter(variable.Item1, variable.Item2);
                }
            }
        }
    }
}