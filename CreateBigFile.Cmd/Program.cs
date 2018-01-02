using System;
using System.Text;
using XorLog.Core;

namespace CreateBigFile.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            var appender = new FileAppender();
            appender.OpenFile("..\\..\\..\\Files4Test\\BigFile.txt");
            const long FILE_SIZE_IN_MB = 101;
            const long MAX_FILE_SIZE_IN_BYTES = FILE_SIZE_IN_MB * 1024 * 1024;
            var sb = new StringBuilder();
            int counter = 0;
            Console.WriteLine("File size to generate: "+FILE_SIZE_IN_MB+" MB");
            while (appender.FileSize < MAX_FILE_SIZE_IN_BYTES)
            {
                sb.Clear();
                for (int i = 0; i < 10; i++)
                {
                    sb.Append(counter + " ");
                }
                appender.AppendLine(sb.ToString());
                counter++;
                decimal progress = (decimal)appender.FileSize / MAX_FILE_SIZE_IN_BYTES * 100;
                Console.Write(string.Format("{0:f1}% \r", progress));
            }
            appender.AppendLine("----END OF FILE -------");
            appender.CloseFile();
        }
    }
}
