using System;
using System.IO;
using NUnit.Framework;

namespace XorLog.Core.Tests
{
    class RawFileReaderExTest : TestUtil
    {

        private RawFileReaderEx _sut;

        [SetUp]
        public void Setup()
        {
            ConfigureLogger();
            _sut = new RawFileReaderEx();
            Log.Debug("current dir: "+Environment.CurrentDirectory);
        }

        [Test]
        public void CreateLinesFromBuffer_When2LinesInCharArray_ThenLinesAreCorrectlySeparated()
        {
            const string LINE_1 = "123456";
            const string LINE_2 = "789";
            const string CONTENT = LINE_1 + "\r\n" + LINE_2 + "\r\n";
            char[] buffer= CONTENT.ToCharArray(); 

            var result = _sut.CreateLinesFromBuffer(buffer, buffer.Length);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(LINE_1, result[0]);
            Assert.AreEqual(LINE_2, result[1]);
        }

        [Test]
        public void ReadLine_WhenReadFirstLineOfFile_ThenLineContentIsValid()
        {
            const string EXPECTED_RESULT = "this is first line";//first line in file
            _sut.Open("5lines.txt");

            var result = _sut.ReadLine();

            _sut.Close();
            Assert.AreEqual(EXPECTED_RESULT, result);
        }
        
        [Test]
        public void ReadBlock_WhenReadFirstLineOfFile_ThenLineContentIsValid()
        {
            const string EXPECTED_RESULT = "this is first line";//first line in file
            _sut.Open("5lines.txt");
            const int BUFFER_SIZE = 20;
            char[] buffer = new char[BUFFER_SIZE];

            var result = _sut.ReadBlock(buffer, BUFFER_SIZE, null);

            _sut.Close();
            Assert.AreEqual(20, result.SizeInBytes);
            Assert.AreEqual(1, result.Content.Count);
            Assert.AreEqual(EXPECTED_RESULT, result.Content[0]);
        }

        [Test]
        public void Open_WhenFileIsDeletedWhileOpen_ThenDoNotExistAnyMore()
        {
            var appender = new Appender();
            const string TEMP_TEST_FILE = "temp222.txt";
            const string NEW_LINE = "new line";
            appender.OpenFile(TEMP_TEST_FILE);
            appender.AppendLine(NEW_LINE);
            appender.CloseFile();

            _sut.Open(TEMP_TEST_FILE);
            File.Delete(TEMP_TEST_FILE); // delete the file when open

            Assert.IsFalse(File.Exists(TEMP_TEST_FILE), "File should be deleted");
        }
    }
}
