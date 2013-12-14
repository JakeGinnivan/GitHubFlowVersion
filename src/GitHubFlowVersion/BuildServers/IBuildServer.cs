using LibGit2Sharp;

namespace GitVersion.BuildServers
{
    public interface IBuildServer
    {
        bool IsRunningInBuildAgent();
        bool IsBuildingAPullRequest(IRepository repository);
        int CurrentPullRequestNo(Branch branch);
        void WriteBuildNumber(SemanticVersion nextBuildNumber);
        void WriteParameter(string variableName, string value);
    }
}