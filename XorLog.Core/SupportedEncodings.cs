using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XorLog.Core
{
    public class SupportedEncodings
    {
        public const string ANSI = "ANSI";
        public const string UTF8 = "UTF8";
        public const string UTF32 = "UTF32";
        public const string ASCII = "ASCII";

        private IEnumerable<EncodingItem> _encodingItems;

        public IEnumerable<EncodingItem> GetEncoderList()
        {
            if (_encodingItems == null)
            {
                _encodingItems = CreateEncoderList();
            }
            return _encodingItems;
        }

        public bool IsValidName(string name)
        {
            bool ret = _encodingItems.Any(x => x.DisplayName == name);
            return ret;
        }

        public EncodingItem GetItem(string name)
        {
            EncodingItem ret = _encodingItems.First(x => x.DisplayName == name);
            return ret;
        }

        public IEnumerable<EncodingItem> CreateEncoderList()
        {
            List<EncodingItem> ret = new List<EncodingItem>();
            ret.Add(new EncodingItem(ANSI, Encoding.Default));
            ret.Add(new EncodingItem(UTF8, Encoding.UTF8));
            ret.Add(new EncodingItem(UTF32, Encoding.UTF32));
            ret.Add(new EncodingItem(ASCII, Encoding.ASCII));
            return ret;
        }

    }
}