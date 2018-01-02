using System.Collections.Generic;
using System.IO;

namespace XorLog.Core
{
    public interface IRawFileReader
    {
        void Open(string path);
        void Close();
        void SetPosition(long offset, SeekOrigin origin);
        long GetPosition();
        ReadBlock ReadBlock(char[] buffer, long count);
        int Peek();
        IList<string> GetEndOfFile(long offsetStart);
        string ReadLine();
        IList<string> CreateLinesFromBuffer(char[] buffer, long length);
    }
}