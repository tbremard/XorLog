using System;
using System.IO;
using System.Threading;

namespace XorLog.Core
{
    public class FileAppender
    {
        private StreamWriter _stream;
        public long FileSize 
        { 
            get
            {
                info.Refresh();
                return info.Length;
            }  
        }
        private FileInfo info;
        public void OpenFile(string fileName)
        {
            _stream = new StreamWriter(fileName, true);
            info = new FileInfo(fileName);
        }

        public void DeleteFile(string fileName)
        {
            File.Delete(fileName);
        }

        public void CloseFile()
        {
            _stream.Close();
            _stream.Dispose();
        }

        public void AppendLines(int nbLines)
        {
            int i = 0;
            while (i < nbLines)
            {
                Console.WriteLine("Line added: " + i);
                AppendLine(i.ToString());
                Thread.Sleep(500);
                i++;
            }
        }

        public void AppendLine(string s)
        {
            _stream.WriteLine(s);
            _stream.Flush();
        }

        public void SetLength(long value)
        {
            _stream.BaseStream.SetLength(value);
        }
    }
}