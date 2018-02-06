using System;
using System.IO;
using System.Text;
using System.Threading;

namespace XorLog.Core
{
    public class Appender
    {
        private FileInfo _info;
        private FileStream _stream;
        public long FileSize 
        { 
            get
            {
                _info.Refresh();
                return _info.Length;
            }  
        }
        public void OpenFile(string fileName)
        {
            _stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete);
            _stream.Seek(0, SeekOrigin.End);
//            _stream = new StreamWriter(fileName, true);
            _info = new FileInfo(fileName);
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
            byte[] toBytes = Encoding.UTF8.GetBytes(s+"\r\n");
            _stream.Write(toBytes, 0, toBytes.Length);
            _stream.Flush();
        }

        public void SetLength(long value)
        {
            _stream.SetLength(value);
        }
    }
}