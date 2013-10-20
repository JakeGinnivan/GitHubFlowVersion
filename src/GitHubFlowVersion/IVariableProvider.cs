using System.Collections.Generic;

namespace GitHubFlowVersion
{
    public interface IVariableProvider
    {
        Dictionary<string, string> GetVariables(SemanticVersion nextBuildNumber);
    }
}