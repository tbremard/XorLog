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
        private IList<string> _rejectionList;
        public IList<string> RejectionList { get { return _rejectionList; }
            set
            {
                Log.Debug("Rejection list is udated");
                _rejectionList = value;
                if (FileIsOpen)
                {
                    _stream.SetPosition(_currentPage.OffsetStart, SeekOrigin.Begin);
                    FillCurrentPage(_currentPage.OffsetStart);
                    OnPageLoaded();
                }
            }
        }

        public long PageSizeInBytes { get { return MAX_PAGE_SIZE_IN_BYTES; } }
        private const double MINIMUM_DELAY_IN_MS = 200;
        private const int MAX_PAGE_SIZE_IN_BYTES = 1000000;
        private const int PAGE_OVERLAPPING_IN_BYTES = 100;
        private IRawFileReader _stream;
        private Page _currentPage;
        private SizeOfFileWatcher _fileWatcher;
        private bool _isReading = true;
        private PageRequest _lastRequest;
        private string _path;

        public bool NewContentIsAvailable { get; private set; }

        public Presenter()
        {
            Log.DebugFormat("Using PageSize: {0} Bytes", PageSizeInBytes);
            Thread t = new Thread(RequestPoller);
            t.Start();
            RejectionList = new List<string>();
        }

        private bool FileIsOpen;
        public void OpenFile(string path)
        {
            _path = path;
            _stream = new RawFileReaderEx();
            _stream.Open(path);
            WatchFile(path);
            OnFileLoaded(path);
            FileIsOpen = true;
        }

        private void OnFileLoaded(string pathFileName)
        {
            if (FileLoaded == null)
            {
                return;
            }
            string path = Path.GetDirectoryName(pathFileName);
            string fileName = Path.GetFileName(pathFileName);
            var arg = new FileLoadedEventArgs(_fileWatcher.SizeOfFile, path, fileName);
            FileLoaded(this, arg);
        }

        private void OnPageLoaded()
        {
            var args = new PageLoadedEventArgs(_currentPage, KindOfPage.Current);
            if (PageLoaded!=null)
            {
                Log.Debug("currentPage: "+_currentPage.ToString());
                Log.Debug("Raising event PageLoaded...");
                PageLoaded(this, args);
                Log.Debug("...event PageLoaded is raised");
            }
        }

        private void WatchFile(string path)
        {
            var fileName = Path.GetFileName(path);
            string directoryName = Path.GetDirectoryName(path);
            _fileWatcher = new SizeOfFileWatcher(directoryName, fileName);
            _fileWatcher.SizeOfFileChanged += SizeOfFileChangedHandler;
            _fileWatcher.Start();
        }

        private void SizeOfFileChangedHandler(object sender, SizeOfFileEventArgs e)
        {
            NewContentIsAvailable = true;
            if (e.CurrentSizeOfFile == 0)
            {
                EmptyPage();
                OnPageLoaded();
                return;
            }
            if (e.CurrentSizeOfFile>e.LastSizeOfFile)
            {
                Log.Debug("size of file increased");
                IList<string> tail = _stream.GetEndOfFile(e.LastSizeOfFile, RejectionList);
                OnTailUpdated(tail);                
            }
            else if (e.CurrentSizeOfFile < e.LastSizeOfFile)
            {
                Log.Debug("size of file decreased");
                _stream.SetPosition(_currentPage.OffsetStart, SeekOrigin.Begin);
                FillCurrentPage(_currentPage.OffsetStart);
                OnPageLoaded();
            }
        }

        private void EmptyPage()
        {
            var noline = new List<string>();
            _currentPage = new Page(0, 0, 0, noline, 0);
        }

        private void OnTailUpdated(IList<string> tail)
        {
            var args = new TailUpdatedEventArgs(tail, _fileWatcher.SizeOfFile);
            if (TailUpdated != null)
            {
                TailUpdated(this, args);
            }
        }

        public void CloseFile()
        {
            Log.Debug("Closing");
            _isReading = false;
            if (_stream==null)
            {
                return;
            }
            _stream.Close();
            _stream = null;
            _fileWatcher.Stop();
            Log.Debug("Closed");
        }

        private void FillCurrentPage(long requestedOffset)
        {
            long offsetStart = _stream.GetPosition();
            char[] buffer = new char[MAX_PAGE_SIZE_IN_BYTES];
            ReadBlock block = _stream.ReadBlock(buffer, MAX_PAGE_SIZE_IN_BYTES, RejectionList);
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
            long delta = Math.Min(MAX_PAGE_SIZE_IN_BYTES, _fileWatcher.SizeOfFile);
            _stream.SetPosition(-1 * delta, SeekOrigin.End);
            _stream.ReadLine(); // discard content till first end of line so no partial line displayed
            long requestedOffset = _fileWatcher.SizeOfFile;
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
            if (offset > _fileWatcher.SizeOfFile)
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
                if (_fileWatcher.SizeOfFile > MAX_PAGE_SIZE_IN_BYTES)
                {
                    long temp = _fileWatcher.SizeOfFile - MAX_PAGE_SIZE_IN_BYTES;
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
            long ret = 0;
            try
            {
                ret = _fileWatcher.SizeOfFile;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
            return ret;
        }

        private int _searchIdCounter = 1;
        public int Search(string searchPattern)
        {
            int ret = _searchIdCounter;
            _searchIdCounter++;
            var textReader = new RawFileReader();
            textReader.Open(_path);
            List<string> linesFound = new List<string>();
            while (!textReader.IsEndOfFile())
            {
                string line = textReader.ReadLine();
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

        public bool DeleteFile()
        {
            int counter = 0;
            while (File.Exists(_path))
            {
                try
                {
                    Log.DebugFormat("Try to delete file: {0}", _path);
                    File.Delete(_path);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
                Thread.Sleep(100);
                if(counter==10)
                    break;
                counter++;
            }
            bool ret = File.Exists(_path);
            return ret;
        }
    }
}