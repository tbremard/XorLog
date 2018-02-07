using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace XorLog.Core.Tests
{
    class PresenterTest : TestUtil
    {
        private ResultOfSearch _resultOfSearch;
        const string TEST_FILE = @"..\src\Files4Test\BigFile.txt";
        const string SAMPLE_FILE = @"..\src\Files4Test\Sample.txt";
        
        private Page _currentPage;
        private Presenter _sut;

        [SetUp]
        public void Setup()
        {
            ConfigureLogger();
            Log.Debug("current directory: "+Environment.CurrentDirectory);
            _sut = new Presenter();
            _sut.PageLoaded += _sut_PageLoaded;
            _sut.SearchIsFinished += SutOnSearchIsFinished;
        }

        private void SutOnSearchIsFinished(object sender, SearchEventArgs searchEventArgs)
        {
            _resultOfSearch = searchEventArgs.ResultOfSearch;
        }

        void _sut_PageLoaded(object sender, PageLoadedEventArgs e)
        {
            _currentPage = e.Content;
            Log.Debug("Page is loaded");
        }

        [Test]
        public void RequestOffset_WhenNextPage_ThenOffsetFollow()
        {
            _sut.OpenFile(TEST_FILE);

            if (_sut.GetFileSize() < 2 * _sut.PageSizeInBytes)
            {
                Assert.Fail("File is not long enough: you should at least have two pages for this test\r\n" + TEST_FILE);
            }
            _sut.GetFirstPage();
            WaitForPage();
            long offsetEndOfFirstPage = _currentPage.OffsetStop;
            var copyReference = _currentPage;
            _currentPage = null;
            _sut.LoadNextPage(copyReference);
            WaitForPage();
            long offsetStartOfSecondPage = _currentPage.OffsetStart;

            Assert.LessOrEqual(offsetStartOfSecondPage, offsetEndOfFirstPage);
        }

        [Test]
        public void ReadLineAtOffset_WhenOffsetIsLastLineOfPreviousPage_ThenContentIsValid()
        {
            _sut.OpenFile(TEST_FILE);
            _sut.GetFirstPage();
            WaitForPage();
            string expectedResult = _currentPage.Lines.Last();
            long offsetEndOfFirstPage = _currentPage.OffsetStop-expectedResult.Length-1;

            var result = _sut.ReadLineAtOffset(offsetEndOfFirstPage);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void GetFileSize_WhenFileIsChanged_ThenSizeIsUpdated()
        {
            _sut.OpenFile(TEST_FILE);
            const string NEW_LINE = "TEST LINE ADDED";
            long  firstSize = _sut.GetFileSize();

            var appender = new Appender();
            appender.OpenFile(TEST_FILE);
            appender.AppendLine(NEW_LINE);
            appender.CloseFile();

            long secondSize = _sut.GetFileSize();
            long diffFileSize = secondSize - firstSize;

            Assert.GreaterOrEqual(diffFileSize, NEW_LINE.Length);
        }

        [Test]
        public void NewContentIsAvailable_WhenLineIsAdded_ThenTrue()
        {
            _sut.OpenFile(TEST_FILE);
            const string NEW_LINE = "TEST LINE ADDED";

            Thread.Sleep(1000);
            var appender = new Appender();
            appender.OpenFile(TEST_FILE);
            appender.AppendLine(NEW_LINE);
            appender.CloseFile();
            Log.Debug("Line is added");
            
            Thread.Yield();
            Thread.Sleep(1000);

            Assert.IsTrue(_sut.NewContentIsAvailable);
        }

        [Test]
        public void TailUpdated_WhenFileIsChanged_ThenTailIsNotified()
        {
            _sut.OpenFile(TEST_FILE);
            _newTail = null;
            _sut.TailUpdated += SutOnTailUpdated; 
            const string NEW_LINE = "TEST LINE ADDED";

            var appender = new Appender();
            appender.OpenFile(TEST_FILE);
            appender.AppendLine(NEW_LINE);
            appender.CloseFile();

            WaitForTail();
            Assert.IsNotNull(_newTail, "New tail should have been set");
            Assert.IsTrue(_newTail.Contains(NEW_LINE), "New line should be contained in tail");
            Assert.AreEqual(1, _newTail.Count, "Invalid number of lines in tail");
        }

        [Test]
        public void Search_WhenPatternIsFound_ThenResultHasCorrectNumberOfLines()
        {
            const int EXPECTED_RESULT = 13;
            const string SEARCH_PATTERN = "permet";
            _sut.OpenFile(SAMPLE_FILE);

            int searchId = _sut.Search(SEARCH_PATTERN);
            WaitForEndOfSearch(searchId);

            Assert.AreEqual(EXPECTED_RESULT, _resultOfSearch.Content.Count);
            DumpList(_resultOfSearch.Content);
        }

        [Test]
        public void PageLoaded_WhenFileIsDeleted_ThenPageIsBlank()
        {
            const string TEMP_TEST_FILE = "temp1.txt";

            var appender = new Appender();
            const string NEW_LINE = "new line";
            appender.OpenFile(TEMP_TEST_FILE);
            appender.AppendLine(NEW_LINE);
            appender.AppendLine(NEW_LINE);
            appender.AppendLine(NEW_LINE);
            appender.CloseFile();

            _sut.OpenFile(TEMP_TEST_FILE);
            _sut.GetFirstPage();
            WaitForPage();
            _currentPage = null;
            File.Delete(TEMP_TEST_FILE);
            Assert.IsFalse(File.Exists(TEMP_TEST_FILE),"File was intended to be deleted, but still present !");
            Log.Debug("File is deleted");
            WaitForPage();

            const int EXPECTED_RESULT = 0;
            Assert.IsNotNull(_currentPage, "Page was not set !");
            Assert.AreEqual(EXPECTED_RESULT, _currentPage.Lines.Count);
        }

        [Test]
        public void PageLoaded_WhenRejectionIsSet_ThenPageHasNoRejectedWord()
        {
            const string TEMP_TEST_FILE = "temp1.txt";
            File.Delete(TEMP_TEST_FILE);
            var appender = new Appender();
            const string REJECTION_WORD = "REJECT";
            const string NEW_LINE_1 = "aaaaaaaaaaaaaa";
            const string NEW_LINE_2 = "bbb" + REJECTION_WORD+"cccc";
            const string NEW_LINE_3 = "cccccccccccccc" + REJECTION_WORD;
            const string NEW_LINE_4 = "zzzzzzzzzzzzzzzzzz";
            appender.OpenFile(TEMP_TEST_FILE);
            appender.AppendLine(NEW_LINE_1);
            appender.AppendLine(NEW_LINE_2);
            appender.AppendLine(NEW_LINE_3);
            appender.AppendLine(NEW_LINE_4);
            appender.CloseFile();

            var rejectionList = new List<string>{REJECTION_WORD};
            _sut.RejectionList = rejectionList;
            _sut.OpenFile(TEMP_TEST_FILE);
            _sut.GetFirstPage();
            WaitForPage();

            const int EXPECTED_RESULT = 2;
            Assert.IsNotNull(_currentPage, "Page was not set !");
            Assert.AreEqual(EXPECTED_RESULT, _currentPage.Lines.Count);
        }

        [Test]
        public void PageLoaded_WhenFileIsTruncated_ThendPageIsCorrectlyUpdated()
        {
            const string TEMP_TEST_FILE = "temp2.txt";

            var appender = new Appender();
            const string NEW_LINE = "new line";
            File.Delete(TEMP_TEST_FILE);
            appender.OpenFile(TEMP_TEST_FILE);
            appender.AppendLine(NEW_LINE);
            appender.AppendLine(NEW_LINE);
            appender.AppendLine(NEW_LINE);

            _sut.OpenFile(TEMP_TEST_FILE);
            _sut.GetFirstPage();
            WaitForPage();
            _currentPage = null;
            int newLength = NEW_LINE.Length;
            appender.SetLength(newLength);
            appender.CloseFile();
            WaitForPage();

            Assert.IsNotNull(_currentPage, "Page was not set");
            const int EXPECTED_RESULT = 1;
            Assert.AreEqual(EXPECTED_RESULT, _currentPage.Lines.Count);
            Assert.AreEqual(NEW_LINE, _currentPage.Lines[0], "invalid content of line");
        }

        private void DumpList(List<string> content)
        {
            foreach (string line in content)
            {
                Console.WriteLine(line);
            }
        }

        private IList<string> _newTail;
        private void SutOnTailUpdated(object sender, TailUpdatedEventArgs args)
        {
            Log.Debug("tail is received");
            _newTail = args.Tail;
        }

        private void WaitForEndOfSearch(int searchId)
        {
            int maxCounter = 10;
            int counter=0;
            do
            {
                while (_resultOfSearch == null)
                {
                    Thread.Sleep(100);
                    if (counter==maxCounter)
                    {
                        throw new TimeoutException("search");
                    }
                    counter++;
                }
            } while (_resultOfSearch.SearchId != searchId);

            Console.WriteLine("Search is finished");            
        }

        private void WaitForPage()
        {
            int counter = 0;
            while (_currentPage == null && counter <10)
            {
                const int SLEEP_TIME_IN_MS = 500;
                Thread.Sleep(SLEEP_TIME_IN_MS);
                Thread.Yield();
                counter++;
            }
            if (_currentPage != null)
            {
                Log.DebugFormat("Page loaded succesfully. TotalSize= {0} bytes", _currentPage.TotalSize);
            }
            else
            {
                Log.Debug("Page loading failed (timeout)");
            }
        }
        private void WaitForTail()
        {
            int counter = 0;
            while (_newTail == null && counter < 10)
            {
                const int SLEEP_TIME_IN_MS = 500;
                Thread.Sleep(SLEEP_TIME_IN_MS);
                Thread.Yield();
                counter++;
            }
            if (_newTail != null)
            {
                Log.Debug("_newTail loaded succesfully. TotalLines= " + _newTail.Count);
            }
            else
            {
                Log.Debug("_newTail loading failed (timeout)");
            }
        }

    }
}
