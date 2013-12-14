using System;

namespace GitHubFlowVersion
{
    class Log : ILog
    {
        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void WriteErrorLine(Exception exception)
        {
            Console.Error.WriteLine(exception);
        }

        public void WriteErrorLine(string error)
        {
            Console.Error.WriteLine(error);
        }
    }
}