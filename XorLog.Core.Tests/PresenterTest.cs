﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NSubstitute;
using NUnit.Framework;

namespace XorLog.Core.Tests
{
    class PresenterTest : TestUtil
    {
        private ResultOfSearch _resultOfSearch;
        const string TEST_FILE = @"D:\prog\design_patterns\XorLog\src\src\Files4Test\BigFile.txt";
        const string SAMPLE_FILE = @"D:\prog\design_patterns\XorLog\src\src\Files4Test\Sample.txt";
        
        private Page _currentPage;
        private Presenter _sut;
        private ILogView _view;

        [SetUp]
        public void Setup()
        {
            ConfigureLogger();
            Log.Debug("current directory: "+Environment.CurrentDirectory);
            _view = Substitute.For<ILogView>();
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

            var appender = new FileAppender();
            appender.OpenFile(TEST_FILE);
            appender.AppendLine(NEW_LINE);
            appender.CloseFile();

            long secondSize = _sut.GetFileSize();
            long diffFileSize = secondSize - firstSize;

            Assert.GreaterOrEqual(diffFileSize, NEW_LINE.Length);
        }

        [Test]
        public void NewContentIsAvailable_WhenFileIsChanged_ThenTrue()
        {
            _sut.OpenFile(TEST_FILE);
            const string NEW_LINE = "TEST LINE ADDED";

            var appender = new FileAppender();
            appender.OpenFile(TEST_FILE);
            appender.AppendLine(NEW_LINE);
            appender.CloseFile();
            
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

            var appender = new FileAppender();
            appender.OpenFile(TEST_FILE);
            appender.AppendLine(NEW_LINE);
            appender.CloseFile();

            Thread.Sleep(1000);
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

//        [Ignore("Requirement canceled")]
        [Test]
        public void PageLoaded_WhenFileIsDeleted_ThenPageIsBlank()
        {
            const string TEMP_TEST_FILE = "temp1.txt";

            var appender = new FileAppender();
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
        public void PageLoaded_WhenFileIsTruncated_ThendPageIsCorrectlyUpdated()
        {
            const string TEMP_TEST_FILE = "temp2.txt";

            var appender = new FileAppender();
            const string NEW_LINE = "new line";
            appender.OpenFile(TEMP_TEST_FILE);
            appender.AppendLine(NEW_LINE);
            appender.AppendLine(NEW_LINE);
            appender.AppendLine(NEW_LINE);

            const int EXPECTED_RESULT = 1;
            _sut.OpenFile(TEMP_TEST_FILE);
            _sut.GetFirstPage();
            WaitForPage();
            _currentPage = null;
            appender.SetLength(5);
            appender.CloseFile();
            WaitForPage();

            Assert.IsNotNull(_currentPage, "Page was not set");
            Assert.AreEqual(EXPECTED_RESULT, _currentPage.Lines.Count);
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
                Log.Debug("Page loaded succesfully. TotalSize= " + _currentPage.TotalSize);
            }
            else
            {
                Log.Debug("Page loading failed ( timeout)");
            }
        }

    }
}
