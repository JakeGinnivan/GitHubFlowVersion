using LibGit2Sharp;

namespace GitHubFlowVersion.BuildServers
{
    public class LocalBuild : IBuildServer
    {
        public bool IsRunningInBuildAgent()
        {
            return false;
        }

        public bool IsBuildingAPullRequest(IRepository repository)
        {
            return false;
        }

        public int CurrentPullRequestNo(Branch branch)
        {
            return 0;
        }

        public void WriteBuildNumber(SemanticVersion nextBuildNumber)
        {
        }

        public void WriteParameter(string variableName, string value)
        {
        }
    }
}