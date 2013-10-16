using System;

namespace GitHubFlowVersion
{
    public class TeamCityVersionWriter
    {
        public static void WriteBuildNumber(SemanticVersion version)
        {
            Console.WriteLine("##teamcity[buildNumber '{0}']", version);
        }

        public static void WriteAssemblyFileVersion(SemanticVersion version)
        {
            Console.WriteLine("##teamcity[setParameter name='GitHubFlowVersion.FileVersion' value='{0}.{1}.{2}']", 
                version.Major, version.Minor, version.Patch);
        }

    }
}