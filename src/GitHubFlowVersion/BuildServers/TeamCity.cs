using System;
using System.Collections;

namespace GitHubFlowVersion.BuildServers
{
    public class TeamCity : IBuildServer
    {
        public bool IsRunningInBuildAgent()
        {
            var isRunningInBuildAgent = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEAMCITY_VERSION"));
            if (isRunningInBuildAgent)
            {
                Console.WriteLine("Executing inside a TeamCity build agent");
            }
            return isRunningInBuildAgent;
        }

        public bool IsBuildingAPullRequest()
        {
            var branchInfo = GetBranchEnvironmentVariable();
            var isBuildingAPullRequest = !string.IsNullOrEmpty(branchInfo) &&
                (branchInfo.ToLower().Contains("/pull/") || branchInfo.ToLower().Contains("/pull-requests/"));
            if (isBuildingAPullRequest)
            {
                Console.WriteLine("This is a pull request build for pull: " + CurrentPullRequestNo());
            }
            return isBuildingAPullRequest;
        }

        public int CurrentPullRequestNo()
        {
            return int.Parse(GetBranchEnvironmentVariable().Split('/')[2]);
        }

        public void WriteBuildNumber(SemanticVersion nextBuildNumber)
        {
            TeamCityVersionWriter.WriteBuildNumber(nextBuildNumber);
        }

        public void WriteParameter(string variableName, string value)
        {
            TeamCityVersionWriter.WriteParameter(variableName.Replace('_', '.'), value);
        }

        string GetBranchEnvironmentVariable()
        {
            foreach (DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                if (((string)de.Key).StartsWith("teamcity.build.vcs.branch.", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("Found Teamcity Branch: {0}", de.Value);
                    return (string)de.Value;
                }
            }

            return null;
        }
    }
}