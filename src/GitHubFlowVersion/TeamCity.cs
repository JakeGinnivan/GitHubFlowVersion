using System;
using System.Collections;

namespace GitHubFlowVersion
{
    public class TeamCity
    {
        public static bool IsRunningInBuildAgent()
        {
            var isRunningInBuildAgent = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEAMCITY_VERSION"));
            if (isRunningInBuildAgent)
            {
                Console.WriteLine("Executing inside a TeamCity build agent");
            }
            return isRunningInBuildAgent;
        }

        public static bool IsBuildingAPullRequest()
        {
            var branchInfo = GetBranchEnvironmentVariable();
            var isBuildingAPullRequest = !string.IsNullOrEmpty(branchInfo) && branchInfo.ToLower().Contains("/pull/");
            if (isBuildingAPullRequest)
            {
                Console.WriteLine("This is a pull request build for pull: " + CurrentPullRequestNo());
            }
            return isBuildingAPullRequest;
        }


        static string GetBranchEnvironmentVariable()
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

        public static int CurrentPullRequestNo()
        {
            return int.Parse(GetBranchEnvironmentVariable().Split('/')[2]);
        }
    }
}