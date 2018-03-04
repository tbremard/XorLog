using System.Windows.Forms;
using log4net;

namespace XorLog.Core
{
    public class CommandLineParameter
    {
        private static readonly ILog Log = LogManager.GetLogger("CommandLineParameter");

        public string File { get; private set; }
        public bool AutoScroll { get; private set; }
        public FormWindowState WindowState { get; private set; }
        public string Encoding { get; private set; }

        public CommandLineParameter(string[] args)
        {
            LogArgs(args);
            SetDefaultValues();
            Parse(args);
        }

        private void Parse(string[] args)
        {
            if (args.Length == 3)
            {
                if (args[1] == "-file")
                {
                    File = args[2];
                }
            }

            if (args.Length == 2)
            {
                File = args[1];
            }
        }

        private void LogArgs(string[] args)
        {
            Log.Debug("Parameters: ");
            foreach (string arg in args)
            {
                Log.Debug(arg);
            }
        }

        private void SetDefaultValues()
        {
            WindowState = FormWindowState.Normal;
            AutoScroll = true;
            Encoding = SupportedEncodings.UTF8;
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