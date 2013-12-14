using System.IO;
using System.Linq;
using System.Text;

namespace GitVersion.OutputStrategies
{
    public class JsonFileOutputStrategy : IOutputStrategy
    {
        public void Write(GitVersionContext context)
        {
            if (string.IsNullOrEmpty(context.Arguments.ToFile)) return;

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("{");
            var variableList = context.Variables.ToArray();
            for (var index = 0; index < variableList.Length; index++)
            {
                var variable = variableList[index];
                stringBuilder.AppendFormat("    \"{0}\": \"{1}\"{2}", variable.Key, variable.Value,
                    index < variableList.Length - 1 ? "," : string.Empty);
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine("}");

            var directoryName = Path.GetDirectoryName(context.Arguments.ToFile);
            if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
            File.WriteAllText(context.Arguments.ToFile, stringBuilder.ToString());
        }
    }
}