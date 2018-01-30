using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;

namespace XorLog.Core
{
    public class RawFileReaderEx : IRawFileReader
    {
        const char LINE_SEPARATOR = '\n';
        const char CARRIAGE_RETURN = '\r';
        private static readonly ILog Log = LogManager.GetLogger("RawFileReaderEx");
        private FileStream _stream;
        private FileInfo _fileInfo;
        private string _path;
        private long _currentPosition;
        private SeekOrigin _currentSeekOrigin;


        public void Open(string path)
        {
            _path = path;
            CheckPath();
            _currentPosition = 0;
        }

        private void OpenFile()
        {
            CheckPath();
            int bufferSize = 1000;
            try
            {
                _stream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite|FileShare.Delete, bufferSize);
                _fileInfo = new FileInfo(_path);
            }
            catch (IOException e)
            {
                Log.Debug(e);
            }
        }

        private void CheckPath()
        {
            if (string.IsNullOrEmpty(_path))
            {
                throw new ArgumentNullException("Path");
            }
            if (!File.Exists(_path))
            {
                throw new FileNotFoundException(_path);
            }
        }

        public void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        private void CloseFile()
        {
            if (_stream!=null)
            {
                _stream.Close();
                _stream.Dispose();
                _stream = null; 
            }
        }

        public void Close()
        {
            _path = null;
        }

        private void ReloadCurrentPosition()
        {
            _stream.Seek(_currentPosition, _currentSeekOrigin);
        }

        public void SetPosition(long offset, SeekOrigin origin)
        {
            _currentPosition = offset;
            _currentSeekOrigin = origin;
        }

        public long GetPosition()
        {
            return _currentPosition;
        }

        public ReadBlock ReadBlock(char[] buffer, long count)
        {
            OpenFile();
            ReloadCurrentPosition();
            byte[] array = new byte[count];
            int nbRead = _stream.Read(array, 0, (int)count);
            string decoded = Encoding.UTF8.GetString(array, 0, nbRead);
            string char2StringTrimmed = decoded.TrimEnd();
            string[] lines = char2StringTrimmed.Split(LINE_SEPARATOR);
            var ret = new ReadBlock();
            ret.Content = lines.Select(x => x.TrimEnd()).ToList();;
            ret.SizeInBytes = nbRead;
            _currentPosition = _stream.Position;
            _currentSeekOrigin = SeekOrigin.Begin;
            CloseFile();
            return ret;
        }

        public IList<string> CreateLinesFromBuffer(char[] buffer, long length)
        {
            string char2String = new string(buffer, 0, (int) length);
            string char2StringTrimmed = char2String.TrimEnd();
            string[] lines = char2StringTrimmed.Split(LINE_SEPARATOR);
            IList<string> ret = lines.Select(x => x.TrimEnd()).ToList();
            return ret;
        }

        public int Peek()
        {
            throw new NotImplementedException("peek not done");
        }

        public IList<string> GetEndOfFile(long offsetStart)
        {
            OpenFile();
            _fileInfo.Refresh();
            if (offsetStart>_fileInfo.Length)
            {
                Log.Error("file reduced size");
                return new List<string>();
            }
            _currentPosition = offsetStart;
            _stream.Seek(offsetStart, SeekOrigin.Begin);
            long bytesToRead = _fileInfo.Length - offsetStart;
            char[] buffer = new char[bytesToRead];
            ReadBlock block= ReadBlock(buffer, bytesToRead);
            CloseFile();
            IList<string> ret = block.Content;
            return ret; 
        }

        public string ReadLine()
        {
            StringBuilder sb = new StringBuilder(100);
            OpenFile();
            ReloadCurrentPosition();
            while (true)
            {
                char c = (char) _stream.ReadByte();
                if (c==CARRIAGE_RETURN || c==LINE_SEPARATOR)
                {
                    break;
                }
                sb.Append(c);
            }
            string ret = sb.ToString();
            _currentPosition = _stream.Position;
            _currentSeekOrigin = SeekOrigin.Begin;
            CloseFile();
            return ret;
        }
    }

    public class ReadBlock
    {
        public int SizeInBytes;
        public IList<string> Content;
    }

}