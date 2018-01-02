using System;

namespace XorLog.WinMain
{
    class SearchRequest
    {
        public DateTime TimeOfRequest { get;private set; }
        public string SearchPattern { get;private set; }

        public SearchRequest(string searchPattern)
        {
            TimeOfRequest = DateTime.Now;
            SearchPattern = searchPattern;
        }
    }
}