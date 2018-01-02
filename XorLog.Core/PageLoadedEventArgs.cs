using System;

namespace XorLog.Core
{
    public class PageLoadedEventArgs : EventArgs
    {
        public Page Content { get; private set; }
        public KindOfPage KindOfPage { get; private set; }

        public PageLoadedEventArgs(Page content, KindOfPage kindOfPage)
        {
            Content = content;
            KindOfPage = kindOfPage;
        }
    }
}