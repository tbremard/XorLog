using System.Windows.Forms;

namespace XorLog.Core
{
    public class CommandLineParameter
    {
        public string File { get; private set; }
        public bool AutoScroll { get; private set; }
        public FormWindowState WindowState { get; private set; }
        public string Encoding { get; private set; }

        public CommandLineParameter(string[] args)
        {
            DefaultValues();

            if (args.Length == 3)
            {
                if (args[1] == "-file")
                {
                    File = args[2];
                }
            }
        }

        private void DefaultValues()
        {
            WindowState = FormWindowState.Normal;
            AutoScroll = true;
            Encoding = "utf-8";
            File = null;
        }

        public CommandLineParameter(string file, bool autoScroll, FormWindowState windowState, string encoding)
        {
            File = file;
            AutoScroll = autoScroll;
            WindowState = windowState;
            Encoding = encoding;
        }
    }
}