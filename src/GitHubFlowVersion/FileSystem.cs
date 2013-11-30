using System.IO;
using System.Security.Cryptography;

namespace GitHubFlowVersion
{
    class FileSystem : IFileSystem
    {
        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return Directory.GetFiles(path, searchPattern, searchOption);
        }

        public void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            File.Copy(sourceFileName, destFileName, overwrite);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllText(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        public void Move(string sourceFileName, string destFileName, bool overwrite)
        {
            if (File.Exists(destFileName) && overwrite)
                File.Delete(destFileName);
            File.Move(sourceFileName, destFileName);
        }
    }
}