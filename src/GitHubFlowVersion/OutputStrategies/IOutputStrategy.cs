using System;
using System.Collections.Generic;

namespace GitHubFlowVersion.OutputStrategies
{
    public interface IOutputStrategy
    {
        void Write(GitHubFlowArguments gitHubFlowConfiguration, IEnumerable<Tuple<string, string>> variables, SemanticVersion nextBuildNumber);
    }
}