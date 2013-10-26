using System.Collections.Generic;
using GitHubFlowVersion.BuildServers;

namespace GitHubFlowVersion.OutputStrategies
{
    public class BuildServerOutputStrategy : IOutputStrategy
    {
        private readonly IBuildServer _buildServer;

        public BuildServerOutputStrategy(IBuildServer buildServer)
        {
            _buildServer = buildServer;
        }

        public void Write(GitHubFlowArguments gitHubFlowConfiguration, Dictionary<string, string> variables, SemanticVersion nextBuildNumber)
        {
            if (_buildServer.IsRunningInBuildAgent())
            {
                _buildServer.WriteBuildNumber(nextBuildNumber);
                foreach (var variable in variables)
                {
                    _buildServer.WriteParameter(variable.Key, variable.Value);
                }
            }
        }
    }
}