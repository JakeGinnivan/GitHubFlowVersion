namespace GitHubFlowVersion.OutputStrategies
{
    public interface IOutputStrategy
    {
        void Write(GitHubFlowVersionContext context);
    }
}