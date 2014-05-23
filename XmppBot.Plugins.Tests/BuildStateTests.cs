using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using XmppBot.Plugins.Deployments;

namespace XmppBot.Plugins.Tests
{
    [TestClass]
    public class BuildStateTests
    {
        [TestMethod]
        public void BuildStartTest()
        {
            var bambooBuild = new BambooConnection();

            var state = bambooBuild.StartBuild("tnew", "qa", "TWEB");

            Assert.AreEqual("2", state);
        }

        [TestMethod]
        public void BuildStateSuccessFulTest()
        {
            var bambooBuild = new BambooConnection();

            var state = bambooBuild.GetBuildState("TNEXWEB-TNEWV45QALIGHT", "2722");

            Assert.AreEqual("Successful", state);
        }

        [TestMethod]
        public void BuildStateFailureTest()
        {
            var bambooBuild = new BambooConnection();

            //var state = bambooBuild.GetBuildState("TNEXWEB-TNEWV45QALIGHT", "2722");
            var state = bambooBuild.GetBuildState("TNST-TNST15QAFULL", "226");

            Assert.AreEqual("Failed", state);
        }

        [TestMethod]
        public void BuildStateUnknownTest()
        {
            var parser = new BambooResultParser();
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<restQueuedBuild>
  <buildState>Success</buildState>
  <progress>
<percentageCompletedPretty>4%</percentageCompletedPretty>
    </progress>
</restQueuedBuild>";
            var actual = parser.ParseBuildStatusResult(xml, "1", "Key");

            Assert.AreEqual("Success", actual);
        }

        [TestMethod]
        public void BuildStateSuccessTest()
        {
            var parser = new BambooResultParser();
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<restQueuedBuild>
  <buildState>Unknown</buildState>
  <progress>
<percentageCompletedPretty>4%</percentageCompletedPretty>
    </progress>
</restQueuedBuild>";
            var actual = parser.ParseBuildStatusResult(xml, "1", "Key");

            Assert.IsTrue(actual.StartsWith("Build"));
        }

        [TestMethod]
        public void GetBuildNumberTest()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<restQueuedBuild planKey=""TNEXWEB-TNEWV45QALIGHT"" buildNumber=""2724"" buildResultKey=""TNEXWEB-TNEWV45QALIGHT-2724"">
  <triggerReason>Manual build</triggerReason>
  <link href=""http://bamboo.tessituranetwork.com:8085/rest/api/latest/result/TNEXWEB-TNEWV45QALIGHT-2724"" rel=""self""/>
</restQueuedBuild>";

            var parser = new BambooResultParser();
            string buildNumber = parser.ParseQueueResult(xml);

            Assert.AreEqual("2724", buildNumber);
        }
    }
}
