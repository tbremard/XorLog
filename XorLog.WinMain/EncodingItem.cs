using System.Text;

namespace XorLog.WinMain
{
    class EncodingItem
    {
        public string DisplayName { get; private set; }
        public Encoding Encoder { get; private set; }

        public EncodingItem(string displayName, Encoding encoder)
        {
            DisplayName = displayName;
            Encoder = encoder;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}