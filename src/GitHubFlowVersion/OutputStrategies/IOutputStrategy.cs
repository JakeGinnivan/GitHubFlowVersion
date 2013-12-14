namespace GitVersion.OutputStrategies
{
    public interface IOutputStrategy
    {
        void Write(GitHubFlowVersionContext context);
    }
}