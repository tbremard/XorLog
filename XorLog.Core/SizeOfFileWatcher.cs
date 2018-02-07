﻿using System;
using System.IO;
using System.Threading;
using log4net;

namespace XorLog.Core
{
    internal class SizeOfFileWatcher
    {
        private readonly ILog _log;
        private readonly string _directoryName;
        private readonly string _fileName;
        private const int DEFAULT_POLL_INTERVAL_IN_MS = 500;
        private bool _isReady = false;
        private Thread _monitorThread;
        public int PollIntervalInMs { get; set; }
        private bool _shouldStop;
        public event EventHandler<SizeOfFileEventArgs> SizeOfFileChanged;
        public SizeOfFileWatcher(string directoryName, string fileName)
        {
            _log = LogManager.GetLogger("SizeOfFileWatcher");
            _directoryName = directoryName;
            _fileName = fileName;
            PollIntervalInMs = DEFAULT_POLL_INTERVAL_IN_MS;
        }

        public void Start()
        {
            _monitorThread = new Thread(MonitorThreadProc)
            {
                IsBackground = true
            };
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
            _isReady = false;
            _monitorThread = null;
        }

        private void MonitorThreadProc()
        {
            Thread.CurrentThread.Name = "FileWatchThread";
            _log.Debug("MonitorThreadProc is started");
            string fullPath = Path.Combine(_directoryName, _fileName);
            var fileInfo = new FileInfo(fullPath);
            long lastSizeOfFile = fileInfo.Length;
            _isReady = true;
            while (!_shouldStop)
            {
                long currentSizeOfFile = GetCurrentSizeOfFile(fileInfo);
                if (currentSizeOfFile != lastSizeOfFile)
                {
                    OnSizeOfFileChanged(lastSizeOfFile, currentSizeOfFile);
                    lastSizeOfFile = currentSizeOfFile;
                }
                Thread.Sleep(PollIntervalInMs);
            }
            _log.Debug("MonitorThreadProc is finished");
        }

        private long GetCurrentSizeOfFile(FileInfo fileInfo)
        {
            long currentLength = 0;
            try
            {
                fileInfo.Refresh();
                currentLength = fileInfo.Length;
            }
            catch (FileNotFoundException)
            {
                _log.Debug("File is not found");
            }
            return currentLength;
        }

        private void OnSizeOfFileChanged(long lastSizeOfFile, long currentSizeOfFile)
        {
            if (SizeOfFileChanged != null)
            {
                var args = new SizeOfFileEventArgs(lastSizeOfFile, currentSizeOfFile);
                _log.Debug("Raising event SizeOfFileChanged...");
                try
                {
                    SizeOfFileChanged(this, args);
                }
                catch (Exception e)
                {
                    _log.Error(e.ToString());
                }
                _log.Debug(".. Event SizeOfFileChanged is raised");                
            }
        }
    }
}