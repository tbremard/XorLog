using NUnit.Framework;

namespace XorLog.Core.Tests
{
    class CommandLineParameterTest
    {
        [Test]
        public void CommandLineParameter_WhenFileIsGiven_ThenFileIsSet()
        {
            string[] args = new string[3];

            args[0] = "prog.exe";
            args[1] = "-file";
            const string FILENAME="file.txt";
            args[2] = FILENAME;

            var result = new CommandLineParameter(args);

            Assert.AreEqual(FILENAME , result.File);
        }

        [Test]
        public void CommandLineParameter_WhenEncodingIsGiven_ThenEncodingIsSet()
        {
            string[] args = new string[3];

            args[0] = "prog.exe";
            args[1] = "-encoding";
            args[2] = SupportedEncodings.UTF32;

            var result = new CommandLineParameter(args);

            Assert.AreEqual(SupportedEncodings.UTF32, result.Encoding, "encoding is incorrect");
        }

        [Test]
        public void CommandLineParameter_WhenFileAndEncodingAreGiven_ThenFileAndEncodingAreSet()
        {
            string[] args = new string[5];

            args[0] = "prog.exe";
            args[1] = "-file";
            const string FILENAME = "file.txt";
            args[2] = FILENAME;
            args[3] = "-encoding";
            args[4] = SupportedEncodings.UTF32;

            var result = new CommandLineParameter(args);

            Assert.AreEqual(FILENAME, result.File);
            Assert.AreEqual(SupportedEncodings.UTF32, result.Encoding, "encoding is incorrect");
        }

    }
}
