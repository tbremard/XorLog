using System;

namespace Xoru.Controls
{
    public class ScrollValueEventArgs : EventArgs
    {
        public int Min { get; private set; }
        public int Max { get; private set; }
        public int Position { get; private set; }

        public ScrollValueEventArgs(int min, int max, int position)
        {
            Min = min;
            Max = max;
            Position = position;
        }
    }
}