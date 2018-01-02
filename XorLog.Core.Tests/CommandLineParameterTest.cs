using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
