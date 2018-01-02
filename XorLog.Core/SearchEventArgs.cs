using System;
using System.Collections.Generic;

namespace XorLog.Core
{
    public class SearchEventArgs: EventArgs
    {
        public ResultOfSearch ResultOfSearch { get; private set; }

        public SearchEventArgs(List<string> content,int searchId)
        {
            ResultOfSearch = new ResultOfSearch {Content = content, SearchId = searchId, IsFinished = true};
        }
    }
}