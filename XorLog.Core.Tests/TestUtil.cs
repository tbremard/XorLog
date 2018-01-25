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

            // On remplace le BasicConfigurator par le XmlConfigurator
            // et on charge la configuration définie dans le fichier log4net.config
            XmlConfigurator.Configure(new FileInfo(configFile));
            Log = LogManager.GetLogger("Test");
        }

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var dir = Path.GetDirectoryName(typeof(RawFileReaderExTest).Assembly.Location);
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