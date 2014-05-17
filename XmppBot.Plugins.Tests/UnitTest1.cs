using System;
using System.Diagnostics;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using XmppBot.Plugins.Deployments;

namespace XmppBot.Plugins.Tests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void VerifyTagDoesNotExist()
        {
            var tagCreator = new TagCreator();
            Assert.IsFalse(tagCreator.TagExists("https://devrepo.tessituranetworkdev.com/svn/tnew2/tags/trunk_v4.5.41.6/"));
        }

        [TestMethod]
        public void VerifyTagDoesExist()
        {
            var tagCreator = new TagCreator();
            Assert.IsFalse(tagCreator.TagExists("https://devrepo.tessituranetworkdev.com/svn/tnew2/tags/trunk_v4.5.41.5/"));
        }
    }
}
