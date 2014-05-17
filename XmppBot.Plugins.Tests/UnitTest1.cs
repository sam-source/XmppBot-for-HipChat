using System;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmppBot.Plugins.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var startup = "svn ls https://devrepo.tessituranetworkdev.com/svn/tnew2/tags/trunk_v4.5.41.5/ --depth empty";
            
            var process = System.Diagnostics.Process.Start(startup);
            
            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();

            Assert.IsTrue(string.IsNullOrWhiteSpace(output));
        }

        [TestMethod]
        public void TestMethod2()
        {
            // var startup = @"""C:\Program Files\TortoiseSVN\bin\svn.exe"" ls ""https://devrepo.tessituranetworkdev.com/svn/tnew2/tags/trunk_v4.5.41.6/"" --depth empty";
            var start = new ProcessStartInfo();
            start.Arguments = @"ls ""https://devrepo.tessituranetworkdev.com/svn/tnew2/tags/trunk_v4.5.41.6/"" --depth empty";
            start.FileName = "svn";
            var process = System.Diagnostics.Process.Start(start);

            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();

            Assert.IsFalse(string.IsNullOrWhiteSpace(output));
        }
    }
}
