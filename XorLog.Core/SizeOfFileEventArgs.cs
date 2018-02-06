using System;

namespace XorLog.Core
{
    internal class SizeOfFileEventArgs: EventArgs
    {
        public long LastSizeOfFile { get; private set; }
        public long CurrentSizeOfFile { get; private set; }

        public SizeOfFileEventArgs(long lastSizeOfFile, long currentSizeOfFile)
        {
            LastSizeOfFile = lastSizeOfFile;
            CurrentSizeOfFile = currentSizeOfFile;
        }
    }
}