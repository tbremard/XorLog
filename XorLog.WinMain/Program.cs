using System;
using System.Windows.Forms;
using log4net;

namespace XorLog.WinMain
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new WinMain());
            }
            catch (Exception ex)
            {
                ILog log = LogManager.GetLogger("Program");
                string message = ex.ToString();
                log.Error(message);
                MessageBox.Show(message);
            }
        }
    }
}
