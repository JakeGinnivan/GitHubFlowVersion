using System.Collections.Generic;

namespace GitVersion
{
    public interface IVariableProvider
    {
        Dictionary<string, string> GetVariables(SemanticVersion nextBuildNumber);
    }
}