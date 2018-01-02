using System;

namespace FileAppender.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName;

            if (args.Length != 2)
            {
                fileName = @"..\..\..\Files4Test\sample.txt";
            }
            else
            {
                fileName = args[1];
            }
            Console.WriteLine("Appending File: " + fileName);
            var appender = new XorLog.Core.FileAppender();
            appender.OpenFile(fileName);
            appender.AppendLines(1000);
            appender.CloseFile();
        }
    }
}