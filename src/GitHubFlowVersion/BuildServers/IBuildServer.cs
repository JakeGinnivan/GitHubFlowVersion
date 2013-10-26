namespace GitHubFlowVersion.BuildServers
{
    public interface IBuildServer
    {
        bool IsRunningInBuildAgent();
        bool IsBuildingAPullRequest();
        int CurrentPullRequestNo();
        void WriteBuildNumber(SemanticVersion nextBuildNumber);
        void WriteParameter(string variableName, string value);
    }
}