using System;
using System.Linq;
using System.Xml.Linq;

namespace XmppBot.Plugins.Deployments
{
    public class BambooResultParser
    {
        public string ParseQueueResult(string xml)
        {
            var xmlDoc = XElement.Parse(xml);

            return xmlDoc.Attribute("buildNumber").Value;
        }

        public string ParseBuildStatusResult(string xml, string buildNumber, string planKey)
        {
            var xmlDoc = XElement.Parse(xml);
            var node = xmlDoc.Element("buildState");

            if (string.Equals(node.Value, "Unknown", StringComparison.InvariantCultureIgnoreCase)) {
                return string.Format(
                    "Build {0} (Plan: {1}) is still in progress. Running for: {2}",
                    buildNumber,
                    planKey,
                    xmlDoc.Element("progress").Descendants("prettyBuildTime").First().Value);
            }

            return node.Value;
        }
    }
}
