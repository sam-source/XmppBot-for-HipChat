using System;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using XmppBot.Common;

namespace XmppBot.Plugins.Tests
{
    [TestClass]
    public class NickNameProviderTests
    {
        public class Stub : BasicFileWrapper
        {
            public string data;

            public Stub() : base("", "")
            {
            }

            public override string GetContents()
            {
                return this.data;
            }

            public override void SaveContents(string contents)
            {
                this.data = contents;
            }
        }

        [TestMethod]
        [TestCategory("XmppBot.User.NickName")]
        public void AddNameTest()
        {
            var systemUnderTest = new NickNameProvider(new Stub());

            systemUnderTest.SaveName("Samuel Menard", "Sam");

            Assert.AreEqual("Sam", systemUnderTest.GetName("Samuel Menard"));
        }

        [TestMethod]
        [TestCategory("XmppBot.User.NickName")]
        public void XmlSaveTest()
        {
            var stub = new Stub();

            var systemUnderTest = new NickNameProvider(stub);

            systemUnderTest.SaveName("Samuel Menard", "Sam");

            Assert.AreEqual("<rootData><nickName fullName=\"Samuel Menard\">Sam</nickName></rootData>", stub.data);
        }

        [TestMethod]
        [TestCategory("XmppBot.User.NickName")]
        public void XmlLoadTest()
        {
            var stub = new Stub();
            
            stub.data = "<rootData><nickName fullName=\"Samuel Menard\">Sam</nickName></rootData>";

            var systemUnderTest = new NickNameProvider(stub);

            Assert.AreEqual("Sam", systemUnderTest.GetName("Samuel Menard"));
        }
    }
}
