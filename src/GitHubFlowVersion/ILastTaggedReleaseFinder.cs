namespace GitHubFlowVersion
{
    public interface ILastTaggedReleaseFinder
    {
        VersionTaggedCommit GetVersion();
    }
}