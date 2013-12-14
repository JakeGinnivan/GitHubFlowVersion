namespace GitVersion
{
    public interface INextVersionTxtFileFinder
    {
        SemanticVersion GetNextVersion(SemanticVersion taggedVersion);
    }
}