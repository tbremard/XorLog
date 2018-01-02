using System;
using System.IO;
using System.Threading;

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
            var fileInfo = new FileInfo(Path.Combine(_directoryName, _fileName));
            long lastLength = fileInfo.Length;
            while (!_shouldStop)
            {
                Thread.Sleep(PollIntervalInMs);
                fileInfo.Refresh();
                long currentLength = fileInfo.Length;
                if (currentLength != lastLength)
                {
                    OnMoreContentAdded(lastLength, currentLength);
                    lastLength = currentLength;
                }
            }
        }

        private void OnMoreContentAdded(long lastLength, long currentLength)
        {
            if (TailChanged != null)
            {
                var args = new TailEventArgs(lastLength, currentLength);
                TailChanged(this, args);
            }
        }
    }
}