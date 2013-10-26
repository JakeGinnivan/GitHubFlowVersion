namespace GitHubFlowVersion.BuildServers
{
    public class LocalBuild : IBuildServer
    {
        public bool IsRunningInBuildAgent()
        {
            return false;
        }

        public bool IsBuildingAPullRequest()
        {
            return false;
        }

        public int CurrentPullRequestNo()
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