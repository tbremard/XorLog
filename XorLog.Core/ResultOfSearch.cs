using System.Collections.Generic;

namespace XorLog.Core
{
    public class ResultOfSearch
    {
        public List<string> Content;
        public int SearchId { get; set; }
        public bool IsFinished;
    }
}