using System;
using System.IO;
using log4net;
using log4net.Config;
using NUnit.Framework;

namespace XorLog.Core.Tests
{
    class TestUtil
    {
        protected ILog Log;

        protected void ConfigureLogger()
        {
            var configFile = Directory.GetCurrentDirectory() + @"\log4net.config";
            var fileInfo = new FileInfo(configFile);
            XmlConfigurator.Configure(fileInfo);
            Log = LogManager.GetLogger("Test");
        }

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            string path = typeof(RawFileReaderExTest).Assembly.Location;
            var dir = Path.GetDirectoryName(path);
            if (dir != null)
            {
                Environment.CurrentDirectory = dir;
                Directory.SetCurrentDirectory(dir);
            }
            else
            {
                throw new Exception(
                    "Path.GetDirectoryName(typeof(TestingWithReferencedFiles).Assembly.Location) returned null");
                
            }
        }
    }
}