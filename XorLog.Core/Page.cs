using System.Collections.Generic;

namespace XorLog.Core
{
    public class Page
    {
        public long OffsetStart { get; private set;}
        public long OffsetStop { get; private set; }
        public long TotalSize { get; private set; }
        public long RequestedOffset{ get; private set; }
        public IList<string> Lines { get; private set; }

        public Page(long offsetStart, long offsetStop, long totalSize, IList<string> lines, long requestedOffset)
        {
            OffsetStart = offsetStart;
            OffsetStop = offsetStop;
            TotalSize = totalSize;
            Lines = lines;
            RequestedOffset = requestedOffset;
        }

        public override string ToString()
        {
            string ret = string.Format("Page: Start:{0}, Stop:{1}, Size:{2}, NbLines:{3}, RequestedOffset:{4}",
                OffsetStart, OffsetStop, TotalSize, Lines.Count, RequestedOffset);
//            if (Lines.Count > 0) commented because dumps private data in log
//            {
//                ret += Environment.NewLine + "First Line: " + Lines.First();
//                ret += Environment.NewLine + "Last Line: " + Lines.Last();
//            }
            return ret;
        }
    }
}