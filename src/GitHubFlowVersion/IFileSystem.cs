using System.IO;

namespace GitHubFlowVersion
{
    public interface IFileSystem
    {
        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
        void Copy(string sourceFileName, string destFileName, bool overwrite);
        void DeleteFile(string path);
        string ReadAllText(string path);
        void WriteAllText(string path, string content);
        void Move(string sourceFileName, string destFileName, bool overwrite);
    }
}