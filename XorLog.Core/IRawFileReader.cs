using System.Collections.Generic;
using System.IO;
using System.Text;

namespace XorLog.Core
{
    public interface IRawFileReader
    {
        void Open(string path);
        void Close();
        void SetPosition(long offset, SeekOrigin origin);
        long GetPosition();
        ReadBlock ReadBlock(char[] buffer, long count, IList<string> _rejectionList);
        int Peek();
        IList<string> GetEndOfFile(long offsetStart, IList<string> _rejectionList);
        string ReadLine();
        IList<string> CreateLinesFromBuffer(char[] buffer, long length);
        void SetEncoding(Encoding itemEncoder);
    }
}