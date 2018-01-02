using System;
using System.Collections.Generic;

namespace XorLog.Core
{
    public class TailUpdatedEventArgs : EventArgs
    {
        public IList<string> Tail { get; private set; }
        public long SizeOfFile{ get; private set; }

        public TailUpdatedEventArgs(IList<string> tail, long sizeOfFile)
        {
            Tail = tail;
            SizeOfFile = sizeOfFile;
        }
        
    }
}