using System.Collections.Generic;

namespace GitHubFlowVersion
{
    public class VariableProvider : IVariableProvider
    {
        public Dictionary<string, string> GetVariables(SemanticVersion nextBuildNumber)
        {
            string numOfCommitsSinceRelease = nextBuildNumber.BuildMetaData == null ? "<unknown>" : nextBuildNumber.BuildMetaData.ToString();

            return new Dictionary<string, string>
            {
                {"GitHubFlowVersion_FullSemVer", nextBuildNumber.ToString()},
                {"GitHubFlowVersion_SemVer", nextBuildNumber.WithBuildMetaData(null).ToString()},
                {"GitHubFlowVersion_AssemblySemVer", nextBuildNumber.WithSuffix(null).WithBuildMetaData(null).ToString()},
                {"GitHubFlowVersion_FourPartVersion", nextBuildNumber.ToVersion().ToString()},
                {"GitHubFlowVersion_Major", nextBuildNumber.Major.ToString() },
                {"GitHubFlowVersion_Minor", nextBuildNumber.Minor.ToString() },
                {"GitHubFlowVersion_Patch", nextBuildNumber.Patch.ToString() },
                {"GitHubFlowVersion_NumCommitsSinceRelease", numOfCommitsSinceRelease},
                {"GitHubFlowVersion_Tag", nextBuildNumber.Suffix}
            };
        }
    }
}