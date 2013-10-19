using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GitHubFlowVersion.OutputStrategies
{
    public class JsonFileOutputStrategy : IOutputStrategy
    {
        public void Write(GitHubFlowArguments gitHubFlowConfiguration, IEnumerable<Tuple<string, string>> variables, SemanticVersion nextBuildNumber)
        {
            if (string.IsNullOrEmpty(gitHubFlowConfiguration.ToFile)) return;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("{");
            var variableList = variables.ToArray();
            for (var index = 0; index < variableList.Length; index++)
            {
                var variable = variableList[index];
                stringBuilder.AppendFormat("    \"{0}\"=\"{1}\"{2}", variable.Item1, variable.Item2,
                    index < variableList.Length ? "," : string.Empty);
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine("}");

            File.WriteAllText(gitHubFlowConfiguration.ToFile, stringBuilder.ToString());
        }
    }
}