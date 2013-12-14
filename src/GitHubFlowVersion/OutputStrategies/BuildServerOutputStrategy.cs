using GitVersion.BuildServers;

namespace GitVersion.OutputStrategies
{
    public class BuildServerOutputStrategy : IOutputStrategy
    {
        private readonly IBuildServer _buildServer;

        public BuildServerOutputStrategy(IBuildServer buildServer)
        {
            _buildServer = buildServer;
        }

        public void Write(GitHubFlowVersionContext context)
        {
            if (_buildServer.IsRunningInBuildAgent())
            {
                _buildServer.WriteBuildNumber(context.NextBuildNumber);
                foreach (var variable in context.Variables)
                {
                    _buildServer.WriteParameter(variable.Key, variable.Value);
                }
            }
        }
    }
}