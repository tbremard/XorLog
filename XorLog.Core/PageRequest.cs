using System;

namespace XorLog.Core
{
    public class PageRequest
    {
        public const long NO_ASK = -1;
        public long LastOffsetAsked = NO_ASK;
        public DateTime LastAskTime;
        public DirectionOfContent LastDirection;
    }
}