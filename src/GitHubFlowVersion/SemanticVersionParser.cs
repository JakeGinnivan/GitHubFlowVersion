using System.Linq;

namespace GitVersion
{
    public class SemanticVersionParser
    {
        public static bool TryParse(string versionString, out SemanticVersion semanticVersion)
        {
            var parts = versionString.Split('-');
            if (parts.Length > 2)
            {
                semanticVersion = null;
                return false;
            }
            var stableParts = parts.First().Split('.');

            if (stableParts.Length > 3)
            {
                semanticVersion = null;
                return false;
            }
            
            int major;
            int minor = 0;
            int patch = 0;

            if (!int.TryParse(stableParts[0], out major))
            {
                semanticVersion = null;
                return false;
            }

            if (stableParts.Length > 1)
            {
                if (!int.TryParse(stableParts[1], out minor))
                {
                    semanticVersion = null;
                    return false;
                }
            }

            if (stableParts.Length > 2)
            {
                if (!int.TryParse(stableParts[2], out patch))
                {
                    semanticVersion = null;
                    return false;
                }
            }

            if (parts.Length > 1)
            {
                //TODO Pre
            }
            semanticVersion = new SemanticVersion(major, minor, patch);
            return true;
        }
    }
}