using System;

namespace XorLog.Core
{
    internal class TailEventArgs: EventArgs
    {
        public long LastLength { get; private set; }
        public long CurrentLength { get; private set; }

        public TailEventArgs(long lastLength, long currentLength)
        {
            LastLength = lastLength;
            CurrentLength = currentLength;
        }
    }
}