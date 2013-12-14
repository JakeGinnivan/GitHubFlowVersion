namespace GitVersion.OutputStrategies
{
    public interface IOutputStrategy
    {
        void Write(GitVersionContext context);
    }
}