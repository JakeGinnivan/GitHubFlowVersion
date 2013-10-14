namespace GitHubFlowVersion
{
    public interface ILastTaggedReleaseFinder
    {
        SemanticVersion GetVersion();
    }
}