using System.Text.RegularExpressions;

namespace GitHubFlowVersion
{
    public static class StringExtensions
    {
        public static string RegexReplace(this string input, string pattern, string replace)
        {
            return Regex.Replace(input, pattern, replace);
        }
    }
}