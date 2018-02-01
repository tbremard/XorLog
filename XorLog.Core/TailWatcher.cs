using System;
using System.IO;
using System.Threading;
using log4net;

namespace XorLog.Core
{
    internal class TailWatcher
    {
        private Thread _monitorThread;
        private readonly string _directoryName;
        private readonly string _fileName;
        private int _pollIntervalInMs = 500;
        public int PollIntervalInMs { get; set; }
        private bool _shouldStop;
        public event EventHandler<TailEventArgs> TailChanged;
        public TailWatcher(string directoryName, string fileName)
        {
            Log = LogManager.GetLogger("TailWatcher");
            _directoryName = directoryName;
            _fileName = fileName;
            PollIntervalInMs = _pollIntervalInMs;
        }

        public void Start()
        {
            _monitorThread = new Thread(MonitorThreadProc);
            _monitorThread.IsBackground = true;
            _shouldStop = false;
            _monitorThread.Start();
        }

        public void Stop()
        {
            _shouldStop = true;
            _monitorThread.Join();
            _monitorThread = null;
        }

        private void MonitorThreadProc()
        {
            Thread.CurrentThread.Name = "TailThread";
            Log.Debug("MonitorThreadProc is started");
            var fileInfo = new FileInfo(Path.Combine(_directoryName, _fileName));
            long lastLength = fileInfo.Length;
            while (!_shouldStop)
            {
                Thread.Sleep(PollIntervalInMs);
                long currentLength = 0; 
                try
                {
                    fileInfo.Refresh();
                    currentLength = fileInfo.Length;
                }
                catch (FileNotFoundException)
                {
                    Log.Debug("File was deleted");
                }
                if (currentLength != lastLength)
                {
                    Log.Debug("!!");
                    OnTailChanged(lastLength, currentLength);
                    lastLength = currentLength;
                }
                else
                {
                    Log.Debug("...");
                }
            }
            Log.Debug("MonitorThreadProc is finished");
        }
        protected ILog Log;

        private void OnTailChanged(long lastLength, long currentLength)
        {
            if (TailChanged != null)
            {
                var args = new TailEventArgs(lastLength, currentLength);
                Log.Debug("Raising event TailChanged...");
                TailChanged(this, args);
                Log.Debug(".. Event TailChanged is raised");                
            }
        }
    }
}