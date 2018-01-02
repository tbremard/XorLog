using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using log4net;

namespace XorLog.Core
{
    public class Presenter
    {
        private static readonly ILog Log = LogManager.GetLogger("Presenter");

        public event EventHandler<FileLoadedEventArgs> FileLoaded;
        public event EventHandler<PageLoadedEventArgs> PageLoaded;
        public event EventHandler<TailUpdatedEventArgs> TailUpdated;
        public event EventHandler<SearchEventArgs> SearchIsFinished; 

        public long PageSizeInBytes { get { return MAX_PAGE_SIZE_IN_BYTES; } }
        private const double MINIMUM_DELAY_IN_MS = 200;
        private const int MAX_PAGE_SIZE_IN_BYTES = 1000000;
        private const int PAGE_OVERLAPPING_IN_BYTES = 100;
        private IRawFileReader _stream;
        private Page _currentPage;
        private TailWatcher _fileWatcher;
        private bool _isReading = true;
        private FileInfo _fileInfo;
        private PageRequest _lastRequest;
        private string _path;

        public bool NewContentIsAvailable { get; private set; }

        public Presenter()
        {
            Log.DebugFormat("Using PageSize: {0} Bytes", PageSizeInBytes);
            Thread t = new Thread(RequestPoller);
            t.Start();
        }

        public void OpenFile(string path)
        {
            _path = path;
            _stream = new RawFileReaderEx();
            _stream.Open(path);
            _fileInfo = new FileInfo(path);
            OnFileLoaded(path);
            WatchFile(path);
        }

        private void OnFileLoaded(string pathFileName)
        {
            if (FileLoaded == null)
            {
                return;
            }
            string path = Path.GetDirectoryName(pathFileName);
            string fileName = Path.GetFileName(pathFileName);
            var arg = new FileLoadedEventArgs(_fileInfo.Length, path, fileName);
            FileLoaded(this, arg);
        }

        private void OnPageLoaded()
        {
            var args = new PageLoadedEventArgs(_currentPage, KindOfPage.Current);
            if (PageLoaded!=null)
            {
                PageLoaded(this, args);
            }
        }

        private void WatchFile(string path)
        {
            var fileName = Path.GetFileName(path);
            string directoryName = Path.GetDirectoryName(path);
            _fileWatcher = new TailWatcher(directoryName, fileName);
            _fileWatcher.TailChanged += TailChangedHandler;
            _fileWatcher.Start();
        }

        private void TailChangedHandler(object sender, TailEventArgs e)
        {
            _fileInfo.Refresh();
            NewContentIsAvailable = true;
            if (e.CurrentLength>e.LastLength)
            {
                IList<string> tail = _stream.GetEndOfFile(e.LastLength);
                OnTailUpdated(tail);                
            }
            else if (e.CurrentLength < e.LastLength)
            {
                FillCurrentPage(_currentPage.OffsetStart);
                OnPageLoaded();
            }
        }

        private void OnTailUpdated(IList<string> tail)
        {
            var args = new TailUpdatedEventArgs(tail, _fileInfo.Length);
            if (TailUpdated != null)
            {
                TailUpdated(this, args);
            }
        }

        public void CloseFile()
        {
            _isReading = false;
            if (_stream==null)
            {
                return;
            }
            _stream.Close();
            _stream = null;
            _fileWatcher.Stop();
        }

        private void FillCurrentPage(long requestedOffset)
        {
            long offsetStart = _stream.GetPosition();
            char[] buffer = new char[MAX_PAGE_SIZE_IN_BYTES];
            ReadBlock block = _stream.ReadBlock(buffer, MAX_PAGE_SIZE_IN_BYTES);
            long currentPageSize = block.SizeInBytes;
            long offsetStop = offsetStart+currentPageSize;
            _currentPage = new Page(offsetStart, offsetStop, currentPageSize, block.Content, requestedOffset);
        }

        public void GetLastPage()
        {
            if (_stream == null)
            {
                return;
            }
            _fileInfo.Refresh();
            long delta = Math.Min(MAX_PAGE_SIZE_IN_BYTES, _fileInfo.Length);
            _stream.SetPosition(-1 * delta, SeekOrigin.End);
            _stream.ReadLine(); // discard content till first end of line so no partial line displayed
            long requestedOffset = _fileInfo.Length;
            FillCurrentPage(requestedOffset);
            NewContentIsAvailable = true;
            OnPageLoaded();
        }

        public void GetFirstPage()
        {
            if (_stream == null)
            {
                return;
            }
            _stream.SetPosition(0, SeekOrigin.Begin);
            long requestedOffset = 0;
            FillCurrentPage(requestedOffset);
            OnPageLoaded();
        }

        public void RequestForOffset(long offset, DirectionOfContent direction)
        {
            Log.Debug("RequestForOFfset " + offset+ " "+ direction);
            if (_stream == null)
            {
                return;
            }
            if (!IsValidOffset(offset))
            {
                return;
            }
            if (IsInCurrentPage(offset))
                return;
            SaveRequest(offset, direction);
        }

        private bool IsValidOffset(long offset)
        {
            bool ret = true;
            if (offset < 0)
            {
                ret = false;
            }
            if (offset>_fileInfo.Length)
            {
                ret = false;
            }
            return ret;
        }

        public bool IsInCurrentPage(long offset)
        {
            if (_currentPage.OffsetStart <= offset && offset <= _currentPage.OffsetStop)
                return true;
            return false;
        }

        public string ReadLineAtOffset(long offset)
        {
            _stream.SetPosition(offset, SeekOrigin.Begin);
            string ret = _stream.ReadLine().TrimEnd();
            return ret;
        }

        private void LoadPageForOffset(PageRequest request)
        {
            if (_stream == null)
            {
                return;
            }
            long requestedOffset = request.LastOffsetAsked;
            DirectionOfContent direction = request.LastDirection;

            long actualOffset=0;
            if (direction == DirectionOfContent.Previous)
            {
                long temp = requestedOffset - MAX_PAGE_SIZE_IN_BYTES + PAGE_OVERLAPPING_IN_BYTES;
                actualOffset = Math.Max(0, temp);
            }
            if (direction == DirectionOfContent.Folowing)
            {
                if (_fileInfo.Length > MAX_PAGE_SIZE_IN_BYTES)
                {
                    long temp = _fileInfo.Length - MAX_PAGE_SIZE_IN_BYTES;
                    actualOffset = Math.Min(requestedOffset - PAGE_OVERLAPPING_IN_BYTES, temp);
                }
                else
                {
                    Log.Error("How the hell did you request this offset ??:" + requestedOffset);
                    actualOffset = requestedOffset - PAGE_OVERLAPPING_IN_BYTES;
                }
            }

            if (actualOffset < 0)
            {
                Log.Error(" BUG : actual offset = "+actualOffset+"reseting it to 0");
                actualOffset = 0;
            }

            _stream.SetPosition(actualOffset, SeekOrigin.Begin);
            //var a = _stream.GetPosition();
            FillCurrentPage(requestedOffset);
            NewContentIsAvailable = true;
            OnPageLoaded();
        }

        private void RequestPoller()
        {
            while (_isReading)
            {
                if (_lastRequest != null)
                {
                    TimeSpan span = DateTime.Now - _lastRequest.LastAskTime;
                    if (span.TotalMilliseconds>MINIMUM_DELAY_IN_MS)
                    {
                        LoadPageForOffset(_lastRequest);
                        _lastRequest = null;
                    }
                    else
                    {
                        Log.Debug("Ignore : must wait, because delay not enough");
                    }
                }
                Thread.Sleep(100);
            }
        }

        private void SaveRequest(long offset, DirectionOfContent direction)
        {
            _lastRequest = new PageRequest();
            _lastRequest.LastOffsetAsked = offset;
            _lastRequest.LastAskTime = DateTime.Now;
            _lastRequest.LastDirection = direction;
        }

        public void LoadNextPage(Page currentPage)
        {
            RequestForOffset(currentPage.OffsetStop+1, DirectionOfContent.Folowing);
        }

        public void LoadPreviousPage(Page currentPage)
        {
            RequestForOffset(currentPage.OffsetStart-1, DirectionOfContent.Previous);
        }

        public long GetFileSize()
        {
            _fileInfo.Refresh();
            return _fileInfo.Length;
        }

        private int _searchIdCounter = 1;
        public int Search(string searchPattern)
        {
            int ret = _searchIdCounter;
            _searchIdCounter++;
            var textReader = new RawFileReader();
            textReader.Open(_path);
            string line;
            List<string> linesFound = new List<string>();
            while (!textReader.IsEndOfFile())
            {
                line = textReader.ReadLine();
                if (Match(searchPattern, line))
                {
                    linesFound.Add(line); 
                }
            }
            textReader.Close();
            OnEndOfSearch(linesFound, ret);
            return ret;
        }

        private void OnEndOfSearch(List<string> linesFound, int searchId)
        {
            Log.DebugFormat("Search is finished with {0} lines found for requestId:{1}", linesFound.Count, searchId);
            var args = new SearchEventArgs(linesFound, searchId);
            if (SearchIsFinished!=null)
            {
                SearchIsFinished(this, args);
            }
            else
            {
                Log.Debug("Client didn't register to event [SearchIsFinished]");
            }
        }

        private bool Match(string searchPattern, string lineOfText)
        {
            bool ret = lineOfText.Contains(searchPattern);
            return ret;
        }
    }
}