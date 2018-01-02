using System;
using System.IO;
using NUnit.Framework;

namespace XorLog.Core.Tests
{
    class RawFileReaderExTest
    {
        private RawFileReaderEx _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new RawFileReaderEx();
            Console.WriteLine("current dire: "+Environment.CurrentDirectory);
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
        public void Open_WhenFileIsDeletedWhileOpen_ThenDoNotExistAnyMore()
        {
            var appender = new FileAppender();
            const string TEMP_TEST_FILE = "temp222.txt";
            const string NEW_LINE = "new line";
            appender.OpenFile(TEMP_TEST_FILE);
            appender.AppendLine(NEW_LINE);
            appender.CloseFile();

            _sut.Open(TEMP_TEST_FILE);
            File.Delete(TEMP_TEST_FILE); // delete the file when open
            _sut.Close();

            Assert.IsFalse(File.Exists(TEMP_TEST_FILE), "File should be deleted");
        }
    }
}
