using System;

namespace Xoru.Controls
{
    public class ScrollLimitEventArgs : EventArgs
    {
        public ScrollLimit Limit { get; private set; }

        public ScrollLimitEventArgs(ScrollLimit limit)
        {
            Limit = limit;
        }
    }
}