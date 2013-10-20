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
                {"GitHubFlowVersion.FullSemVer", nextBuildNumber.ToString()},
                {"GitHubFlowVersion.SemVer", nextBuildNumber.WithBuildMetaData(null).ToString()},
                {"GitHubFlowVersion.FourPartVersion", nextBuildNumber.ToVersion().ToString()},
                {"GitHubFlowVersion.Major", nextBuildNumber.Major.ToString() },
                {"GitHubFlowVersion.Minor", nextBuildNumber.Minor.ToString() },
                {"GitHubFlowVersion.Patch", nextBuildNumber.Patch.ToString() },
                {"GitHubFlowVersion.NumCommitsSinceRelease", numOfCommitsSinceRelease},
                {"GitHubFlowVersion.Tag", nextBuildNumber.Suffix}
            };
        }
    }
}