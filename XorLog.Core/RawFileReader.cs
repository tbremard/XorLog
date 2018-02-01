using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;

namespace XorLog.Core
{
    public class RawFileReader : IRawFileReader
    {
        private StreamReader _stream;
        private static readonly ILog Log = LogManager.GetLogger("RawFileReaderEx");

        public void Open(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("Path");
            }
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            int bufferSize = 1000;
            FileStream fs;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, bufferSize);
            }
            catch (IOException e)
            {
                Log.Debug(e);
                return;
            }
            _stream = new StreamReader(fs);

        }

        public void Close()
        {
            _stream.Close();
            _stream.Dispose();
            _stream = null; 
        }

        public void SetPosition(long offset, SeekOrigin origin)
        {
            _stream.BaseStream.Seek(offset, origin);
            _stream.DiscardBufferedData();
        }

        public long GetPosition()
        {
            long ret = _stream.BaseStream.Position;
            return ret;
        }

        public ReadBlock ReadBlock(char[] buffer, long count, IList<string> _rejectionList)
        {
            throw new NotImplementedException();
            var ret = new ReadBlock();
            return ret;
        }

        public int Peek()
        {
            return _stream.Peek();
        }

        public IList<string> GetEndOfFile(long offsetStart, IList<string> _rejectionList)
        {
            _stream.BaseStream.Seek(offsetStart, SeekOrigin.Begin);
            _stream.DiscardBufferedData();
            var tail = new List<string>();
            while (_stream.Peek() >= 0)
            {
                string line = _stream.ReadLine();
                tail.Add(line);
            }
            return tail;
        }

        public string ReadLine()
        {
            string ret = _stream.ReadLine();
            return ret;
        }

        public bool IsEndOfFile()
        {
            bool ret = _stream.EndOfStream;
            return ret;
        }

        public IList<string> CreateLinesFromBuffer(char[] buffer , long length)
        {
            string a = new string(buffer, 0, (int)length);
            IList<string> ret = a.Split('\n').Select(x => x.TrimEnd()).ToList();
            return ret;
        }

    }
}