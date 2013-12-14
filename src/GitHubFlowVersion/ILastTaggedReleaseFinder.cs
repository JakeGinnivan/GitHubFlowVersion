namespace GitVersion
{
    public interface ILastTaggedReleaseFinder
    {
        VersionTaggedCommit GetVersion();
    }
}