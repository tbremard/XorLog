using System;
using System.IO;
using System.Threading;
using log4net;

namespace XorLog.Core
{
    internal class TailWatcher
    {
        private readonly ILog _log;
        private readonly string _directoryName;
        private readonly string _fileName;
        private const int DEFAULT_POLL_INTERVAL_IN_MS = 500;
        private bool _isReady = false;
        private Thread _monitorThread;
        public int PollIntervalInMs { get; set; }
        private bool _shouldStop;
        public event EventHandler<TailEventArgs> TailChanged;
        public TailWatcher(string directoryName, string fileName)
        {
            _log = LogManager.GetLogger("TailWatcher");
            _directoryName = directoryName;
            _fileName = fileName;
            PollIntervalInMs = DEFAULT_POLL_INTERVAL_IN_MS;
        }

        public void Start()
        {
            _monitorThread = new Thread(MonitorThreadProc);
            _monitorThread.IsBackground = true;
            _shouldStop = false;
            _monitorThread.Start();
            WaitToBeReady();
        }

        private void WaitToBeReady()
        {
            while (!_isReady)
            {
                Thread.Sleep(10);
            }
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
            _log.Debug("MonitorThreadProc is started");
            var fileInfo = new FileInfo(Path.Combine(_directoryName, _fileName));
            long lastLength = fileInfo.Length;
            _isReady = true;
            while (!_shouldStop)
            {
                long currentLength = 0; 
                try
                {
                    fileInfo.Refresh();
                    currentLength = fileInfo.Length;
                }
                catch (FileNotFoundException)
                {
                    _log.Debug("File was deleted");
                }
                if (currentLength != lastLength)
                {
                    _log.Debug("!!");
                    OnTailChanged(lastLength, currentLength);
                    lastLength = currentLength;
                }
                else
                {
                    _log.Debug("...");
                }
                Thread.Sleep(PollIntervalInMs);
            }
            _log.Debug("MonitorThreadProc is finished");
        }

        private void OnTailChanged(long lastLength, long currentLength)
        {
            if (TailChanged != null)
            {
                var args = new TailEventArgs(lastLength, currentLength);
                _log.Debug("Raising event TailChanged...");
                TailChanged(this, args);
                _log.Debug(".. Event TailChanged is raised");                
            }
        }
    }
}