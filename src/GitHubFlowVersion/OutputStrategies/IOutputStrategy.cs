using System.Collections.Generic;

namespace GitHubFlowVersion.OutputStrategies
{
    public interface IOutputStrategy
    {
        void Write(GitHubFlowArguments gitHubFlowConfiguration, Dictionary<string, string> variables, SemanticVersion nextBuildNumber);
    }
}