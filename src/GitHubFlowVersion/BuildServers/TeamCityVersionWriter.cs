﻿using System;

namespace GitVersion.BuildServers
{
    public class TeamCityVersionWriter
    {
        public static void WriteBuildNumber(SemanticVersion version)
        {
            Console.WriteLine("##teamcity[buildNumber '{0}']", version);
        }

        public static void WriteParameter(string name, string value)
        {
            Console.WriteLine("##teamcity[setParameter name='system.{0}' value='{1}']", name, value);
        }
    }
}