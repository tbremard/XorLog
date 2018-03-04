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
            const string KEY_FILE = "-file";
            const string KEY_ENCODING = "-encoding";

            if (args.Length == 2)
            {
                File = args[1];
                return;
            }
            if (args.Length > 2)
            {
                // iterate on couple argName/argValue
                int index = 1;
                while (index < args.Length)
                {
                    string currentKey = args[index];
                    if (currentKey == KEY_FILE)
                    {
                        index++;
                        File = args[index];
                    }
                    if (currentKey == KEY_ENCODING)
                    {
                        index++;
                        Encoding = args[index];
                    }
                    index++;
                }
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