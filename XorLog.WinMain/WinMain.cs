﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using log4net;
using log4net.Config;
using XorLog.Core;
using Xoru.Controls;

namespace XorLog.WinMain
{
    public partial class WinMain : Form, ILogView
    {
        private int MINIMUM_TIME_IN_MS = 300;
        private ILog _log;
        Presenter _presenter;
        private Page _currentPage;
        private SearchRequest _lastRequest;
        private Encoding _selectedEncoder;

        public WinMain()
        {
            Thread.CurrentThread.Name = "MainThread";
            ConfigureLogger();
            InitializeComponent();
        }

        private void WinMain_Shown(object sender, EventArgs e)
        {
            AddLink();
            _presenter = new Presenter();
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            var parameters = new CommandLineParameter(commandLineArgs);
            WindowState = parameters.WindowState;
            chkAutoScroll.Checked = parameters.AutoScroll;
            SetEventHandlers();
            PopulateEncodings(parameters.Encoding);

            if (parameters.File != null && File.Exists(parameters.File))
            {
                OpenFileAndShowPage(parameters.File);
            }
            else
            {
                ClearScreen();
            }
        }

        private void PopulateEncodings(string startupEncoder)
        {
            var encodings = new SupportedEncodings();
            IEnumerable<EncodingItem> list = encodings.GetEncoderList();
            foreach (EncodingItem item in list)
            {
                lstEncoding.Items.Add(item);
            }
            if (encodings.IsValidName(startupEncoder))
            {
                EncodingItem x = encodings.GetItem(startupEncoder);
                lstEncoding.SelectedItem = x;
            }
            else
            {
                lstEncoding.SelectedIndex = 0;
            }
        }

        private void SetEventHandlers()
        {
            _reader.scrollMaster.Scroll += scrollMaster_Scroll;
            _reader.lstPageContent.ScrollLimitReached += LstFileContentScrollLimitReached;
            _reader.lstPageContent.ScrollValueChanged += LstFileContentOnScrollValueChanged;
            _presenter.FileLoaded += _presenter_FileLoaded;
            _presenter.PageLoaded += PresenterOnPageLoaded;
            _presenter.TailUpdated += _presenter_TailUpdated;
            _presenter.SearchIsFinished += _presenter_SearchIsFinished;
        }

        private void AddLink()
        {
            LinkLabel.Link link = new LinkLabel.Link();
            link.LinkData = "https://xoru.eu";
            linkXoru.Links.Add(link);
        }

        void _presenter_SearchIsFinished(object sender, SearchEventArgs e)
        {
            DisplayResultOfSearch(e.ResultOfSearch);
        }

        private void DisplayResultOfSearch(ResultOfSearch resultOfSearch)
        {
            if (InvokeRequired)
            {
                Action<ResultOfSearch> delegateFunc = DisplayResultOfSearch;
                object[] args = new Object[1] { resultOfSearch };
                Invoke(delegateFunc, args);
                return;
            }
            lstSearchResult.Items.Clear();
            lstSearchResult.Items.AddRange(resultOfSearch.Content.ToArray());
        }

        private void OpenFileAndShowPage(string file)
        {
            try
            {
                _log.Debug("Opening file: "+file);
                txtSearch.Enabled = true;
                _reader.Enabled = true;
                _presenter.OpenFile(file);
                if (chkAutoScroll.Checked)
                {
                    _presenter.GetLastPage();
                    _reader.ShowLastLine();
                }
                else
                {
                    _presenter.GetFirstPage();
                    _reader.ShowFirstLine();
                }
            }
            catch (Exception e)
            {
                ClearScreen();
                string msg = e.ToString();
                Debug(msg);
                MessageBox.Show(msg);
            }
        }

        void _presenter_TailUpdated(object sender, TailUpdatedEventArgs e)
        {
            AppendLines(e.Tail);
            ShowSizeOfFile(e.SizeOfFile);
        }

        private void LstFileContentOnScrollValueChanged(object sender, ScrollValueEventArgs args)
        {
            string s = string.Format("SlaveScroll Changed: Min:{0} Max:{1} Position:{2}", args.Min, args.Max, args.Position);
            Debug(s);
            int length = args.Max - args.Min;
            if (length == 0)
            {
                _log.Error("problem because args.Max - args.Min=0 => aborting LstFileContentOnScrollValueChanged()");
                return;
            }
            decimal positionRatio =((decimal)  args.Position)/length;
            decimal offsetInPage = _currentPage.TotalSize* positionRatio;
            long offsetCalculated = _currentPage.OffsetStart + (long)Math.Floor(offsetInPage);
            Debug("Setting scrollMaster.Value =" + offsetCalculated);
            if (IsOutOfRange(offsetCalculated, _reader.scrollMaster))
            {
                _log.Error("offset is out of range");
                return;
            }
            _reader.scrollMaster.Value = (int)offsetCalculated;
        }

        private bool IsOutOfRange(long value, VScrollBar scroll)
        {
            bool ret = value < scroll.Minimum || scroll.Maximum < value;
            return ret;
        }

        private void PresenterOnPageLoaded(object sender, PageLoadedEventArgs args)
        {
            Debug("PageLoaded:" + args.KindOfPage + " " + args.Content);
            if (args.KindOfPage == KindOfPage.Current)
            {
                _currentPage = args.Content;
                DisplayLinesOfCurrentPage();
            }
            long size = _presenter.GetFileSize();
            ShowSizeOfFile(size);

        }

        void _presenter_FileLoaded(object sender, FileLoadedEventArgs e)
        {
            Debug(string.Format("File is loaded: {0} Size: {1} bytes", e.FileName, e.TotalSize));
            btnStart.Enabled = true;
            btnEnd.Enabled = true;
            _presenter.SetEncoding(_selectedEncoder);
            _reader.SetScrollMaster(e.TotalSize);
            txtFilePath.Text = Path.Combine(e.Path, e.FileName);
            ShowSizeOfFile(e.TotalSize);
        }

        private void ShowSizeOfFile(long sizeOfFileInBytes)
        {
            if (isClosing)
                return;

            if (InvokeRequired)
            {
                Delegate func = new Action<long>(ShowSizeOfFile);
                object[] args = { sizeOfFileInBytes };
                Invoke(func, args);
                return;
            }

            lblSizeInBytes.Text = BytesSeparated(sizeOfFileInBytes, ' ') + " Bytes";
            lblSizeReadable.Text = BytesToString(sizeOfFileInBytes);
            _reader.SetScrollMaster(sizeOfFileInBytes);

        }

        private string BytesSeparated(long number, char separator)
        {
            IEnumerable <char> snumber = number.ToString().Reverse();
            StringBuilder ouptut = new StringBuilder();
            int counter = 1;
            foreach (char c in snumber)
            {
                ouptut.Append(c);
                if (counter % 3 == 0)
                {
                    ouptut.Append(separator);
                }
                counter++;
            }
            string ret = new string(ouptut.ToString().Reverse().ToArray());
            return ret;
        }

        static String BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return "0 B";
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            string numberToDisplay = (Math.Sign(byteCount) * num).ToString();
            string unitToDisplay = suf[place];
            string ret = numberToDisplay + " "+unitToDisplay;
            return ret;
        }

        void LstFileContentScrollLimitReached(object sender, ScrollLimitEventArgs e)
        {
            Debug("Limit reached:" + e.Limit);
            switch (e.Limit)
            {
                case ScrollLimit.FirstLine:
                    _presenter.LoadPreviousPage(_currentPage);
                    break;
                case ScrollLimit.LastLine:
                    _presenter.LoadNextPage(_currentPage);
                    break;
            }
        }

        private void DisplayLinesOfCurrentPage()
        {
            if (isClosing)
                return;

            if (InvokeRequired)
            {
                Delegate func = new Action(DisplayLinesOfCurrentPage);
                Invoke(func, null);
                return;
            }
            IEnumerable lines = _currentPage.Lines;
            _reader.lstPageContent.BeginUpdate();
            _reader.lstPageContent.Items.Clear();
            foreach (var line in lines)
            {
                _reader.lstPageContent.Items.Add(line);
            }
            if (chkAutoScroll.Checked)
            {
                _reader.lstPageContent.SelectedIndex = _reader.lstPageContent.Items.Count - 1;
                _reader.lstPageContent.SelectedIndex = -1;
            }
            _reader.lstPageContent.EndUpdate();
            AdjustSlaveScrollToOffset(_currentPage.RequestedOffset);
        }

        public bool BtnStartEnabled
        {
            get { return btnStart.Enabled; }
            set { btnStart.Enabled = value; }
        }
        public bool BtnEndEnabled
        {
            get { return btnEnd.Enabled; }
            set { btnEnd.Enabled = value; }
        }

        private bool isClosing = false;

        private void AppendLines(IList<string> tail)
        {
            if (isClosing)
                return;
            if (InvokeRequired)
            {
                Delegate func = new Action<List<string>>(AppendLines);
                object[] args = { tail };
                BeginInvoke(func, args);
                return;
            }

            _reader.lstPageContent.BeginUpdate();
            foreach (var line in tail)
            {
                _reader.lstPageContent.Items.Add(line);
            }
            if (chkAutoScroll.Checked)
            {
                _reader.ShowLastLine();
            }
            _reader.lstPageContent.EndUpdate();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            chkAutoScroll.Checked = false;
            _presenter.GetFirstPage();
            _reader.ShowFirstLine();
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            _presenter.GetLastPage();
            _reader.ShowLastLine();
        }

        private void DragDropHandler(object sender, DragEventArgs e)
        {
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string file = fileList[0]; 
            OpenFileAndShowPage(file);
        }

        private void DragEnterHandler(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void scrollMaster_Scroll(object sender, ScrollEventArgs e)
        {
            string s = string.Format("MasterScroll changed: oldvalue:{1} newvalue:{0} ScrollOrientation:{2} Type:{3} ", e.NewValue.ToString("D5"), e.OldValue.ToString("D5"), e.ScrollOrientation, e.Type);
            Debug(s);
            long newOffset = e.NewValue;
            if (_presenter.IsInCurrentPage(newOffset))
            {
                AdjustSlaveScrollToOffset(newOffset);
            }
            else
            {
                DirectionOfContent direction;
                if (newOffset > _currentPage.OffsetStop)
                {
                    direction = DirectionOfContent.Folowing;
                }
                else
                {
                    direction = DirectionOfContent.Previous;
                }
                _presenter.RequestForOffset(newOffset, direction);
            }
            if (_reader.IsMasterScrollAtEnd(e.NewValue))
            {
                Debug("end of file reached!");
                _reader.ShowLastLine();
            }
        }

        private void AdjustSlaveScrollToOffset(long requestedOffset)
        {
            if (_currentPage.TotalSize == 0 || requestedOffset == 0 || _reader.lstPageContent.Items.Count==0)
            {
                _reader.lstPageContent.TopIndex = 0;
                return;
            }
            long offsetInCurrentPage = requestedOffset - _currentPage.OffsetStart;
            decimal positionRatio = ((decimal) offsetInCurrentPage) / _currentPage.TotalSize;
            int lineIndexToShow = Math.Min((int)(positionRatio * _currentPage.Lines.Count), _currentPage.Lines.Count-1);
            string lineContent = _reader.lstPageContent.Items[lineIndexToShow].ToString();
            Debug("Adjusting scroll slave to line index: " + lineIndexToShow + ". Line content: [" + lineContent+"]");
            _reader.lstPageContent.TopIndex = lineIndexToShow;
        }

        private void WinMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosing = true;
            _presenter.CloseFile();
        }

        private void Debug(string msg)
        {
            _log.Debug(msg);
        }

        private void ConfigureLogger()
        {
            var configFile = Directory.GetCurrentDirectory() + @"\log4net.config";

            // On remplace le BasicConfigurator par le XmlConfigurator
            // et on charge la configuration définie dans le fichier log4net.config
            XmlConfigurator.Configure(new FileInfo(configFile));
            _log = LogManager.GetLogger("WinMain");
        }

        private void WinMain_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            _reader.Clear();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            RequestSearch(txtSearch.Text);
        }

        private void RequestSearch(string searchPattern)
        {
            lstSearchResult.Items.Clear();
            if (string.IsNullOrEmpty(searchPattern))
                return;
            if (searchPattern.Length < 2)
                return;
            var request = new SearchRequest(searchPattern);
            _lastRequest = request;
        }

        private void tmrSearchRequest_Tick(object sender, EventArgs e)
        {
            if (_lastRequest == null)
            {
                return;
            }
            if (EnoughTimeEllapsed())
            {
                _presenter.Search(_lastRequest.SearchPattern);
                _lastRequest = null;
            }
        }

        private bool EnoughTimeEllapsed()
        {
            TimeSpan ellapsedTime = DateTime.Now - _lastRequest.TimeOfRequest;
            bool ret = ellapsedTime.TotalMilliseconds > MINIMUM_TIME_IN_MS;
            return ret;
        }

        private void linkXoru_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string uri = e.Link.LinkData as string;
            Process.Start(uri);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _presenter.CloseFile();
            ClearScreen();
        }

        private void ClearScreen()
        {
            _reader.Clear();
            _reader.Enabled = false;
            btnEnd.Enabled = false;
            btnStart.Enabled = false;
            txtFilePath.Text = string.Empty;
            txtSearch.Text = string.Empty;
            txtSearch.Enabled = false;
            lstSearchResult.Items.Clear();
            lblSizeInBytes.Text = string.Empty;
            lblSizeReadable.Text = string.Empty;
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            var rejection = new ViewListEditor();
            rejection.SetTitle("Rejection");
            rejection.SetInitialList(_presenter.RejectionList);
            DialogResult x = rejection.ShowDialog();
            if (x == DialogResult.OK)
            {
                _presenter.RejectionList = rejection.ListOfWords;
                _reader.ShowLastLine();//assume you look at last line but could be something else ....
            }
        }

        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            _presenter.DeleteFile();
        }

        private void lstEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = lstEncoding.SelectedItem as EncodingItem;
            _log.Debug("User selected encoding: "+item.DisplayName);
            _selectedEncoder = item.Encoder;
            _presenter.SetEncoding(item.Encoder);
        }
    }
}
