namespace GitHubFlowVersion
{
    public class TeamCityVersionWriter
    {
        public static string WriteBuildNumber(SemanticVersion version)
        {
            return string.Format("##teamcity[buildNumber '{0}']", version);
        }
    }
}