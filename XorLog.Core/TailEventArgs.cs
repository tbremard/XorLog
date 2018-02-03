using System;

namespace XorLog.Core
{
    internal class TailEventArgs: EventArgs
    {
        public long LastSizeOfFile { get; private set; }
        public long CurrentSizeOfFile { get; private set; }

        public TailEventArgs(long lastSizeOfFile, long currentSizeOfFile)
        {
            LastSizeOfFile = lastSizeOfFile;
            CurrentSizeOfFile = currentSizeOfFile;
        }
    }
}