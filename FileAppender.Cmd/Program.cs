using System;
using XorLog.Core;

namespace FileAppender.Cmd
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("usage: <mode(NORMAL|CRAZY)> <path of file to be appended>");
                return;
            }
            string mode = args[0];
            string fileName = args[1];
            Console.WriteLine("Appending File: " + fileName);
            var appender = new Appender();
            appender.OpenFile(fileName);
            if (mode == "NORMAL")
            {
                ModeNormal(appender);
            }
            else if(mode=="CRAZY")
            {
                ModeCrazy(appender);                
            }
            appender.CloseFile();
        }

        private static void ModeCrazy(Appender appender)
        {
            var rand = new Random();
            while (true)
            {
                int target = rand.Next(100, 1000);
                for (int i = 0; i < target; i++)
                {
                    appender.AppendLine("aaaaaaaaaaaaaaaa" + i);
                }
                appender.SetLength(10);
                
            }
        }

        private static void ModeNormal(Appender appender)
        {
            appender.AppendLines(1000);
        }
    }
}