using System;

namespace XorLog.Core
{
    public class FileLoadedEventArgs : EventArgs
    {
        public long TotalSize { get; private set; }
        public string Path { get; private set; }
        public string FileName { get; private set; }

        public FileLoadedEventArgs(long totalSize, string path, string fileName)
        {
            TotalSize = totalSize;
            Path = path;
            FileName = fileName;
        }
    }
}