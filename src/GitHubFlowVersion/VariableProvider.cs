using System;
using System.Collections.Generic;

namespace GitHubFlowVersion
{
    public class VariableProvider : IVariableProvider
    {
        public const string FullSemVer = "GitHubFlowVersion_FullSemVer";
        public const string SemVer = "GitHubFlowVersion_SemVer";
        public const string AssemblyVersion = "GitHubFlowVersion_AssemblyVersion";
        public const string AssemblyFileVersion = "GitHubFlowVersion_AssemblyFileVersion";
        public const string AssemblyInformationalVersion = "GitHubFlowVersion_AssemblyInformationalVersion";
        public const string FourPartVersion = "GitHubFlowVersion_FourPartVersion";
        public const string Major = "GitHubFlowVersion_Major";
        public const string Minor = "GitHubFlowVersion_Minor";
        public const string Patch = "GitHubFlowVersion_Patch";
        public const string NumCommitsSinceRelease = "GitHubFlowVersion_NumCommitsSinceRelease";
        public const string Tag = "GitHubFlowVersion_Tag";

        public Dictionary<string, string> GetVariables(SemanticVersion nextBuildNumber)
        {
            var numOfCommitsSinceRelease = nextBuildNumber.BuildMetaData == null ? "<unknown>" : nextBuildNumber.BuildMetaData.ToString();

            var fullSemanticVersion = nextBuildNumber.ToString();
            return new Dictionary<string, string>
            {
                {FullSemVer, fullSemanticVersion},
                {SemVer, nextBuildNumber.WithBuildMetaData(null).ToString()},
                {AssemblyVersion, new Version(nextBuildNumber.Major, nextBuildNumber.Minor, 0, 0).ToString()},
                {AssemblyFileVersion, nextBuildNumber.WithSuffix(null).ToVersion().ToString()},
                {AssemblyInformationalVersion, fullSemanticVersion},
                {FourPartVersion, nextBuildNumber.ToVersion().ToString()},
                {Major, nextBuildNumber.Major.ToString()},
                {Minor, nextBuildNumber.Minor.ToString()},
                {Patch, nextBuildNumber.Patch.ToString()},
                {NumCommitsSinceRelease, numOfCommitsSinceRelease},
                {Tag, nextBuildNumber.Suffix}
            };
        }
    }
}