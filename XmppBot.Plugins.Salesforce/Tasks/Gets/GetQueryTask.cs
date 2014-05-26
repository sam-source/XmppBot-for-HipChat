using System;
using System.Linq;
using System.Text;

using TND;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks
{
    class GetQueryTask : SimpleTaskBase
    {
        public GetQueryTask(string pluginName)
            : base(pluginName, "get-query") { }

        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            //tnd-get-query type=tnew version=4.5 location=ramp config=live

            var client = new TNEWProvider();
            var args = taskInfo.Args;
            var type = args.FirstOrDefault(a => a.StartsWith("type"));

            ApplicationType appType = ApplicationType.TNEW;

            if (!string.IsNullOrWhiteSpace(type)) {
                if (string.Equals(type, "type=tnst", StringComparison.InvariantCultureIgnoreCase)) {
                    appType = ApplicationType.TNST;
                }
            }

            type = args.FirstOrDefault(a => a.StartsWith("version"));

            if (string.IsNullOrWhiteSpace(type)) {
                return "Filter 'version' is required. Example: 'version=4.5'";
            }

            System.Version version = new System.Version(type.Substring(type.IndexOf("=") + 1));

            type = args.FirstOrDefault(a => a.StartsWith("config"));

            if (string.IsNullOrWhiteSpace(type)) {
                return "Filter 'config' is required. Example: 'config=live'";
            }

            TND.ConfigurationType configType = ConfigurationType.QA;

            var configTypeTemp = type.Substring(type.IndexOf("=") + 1);

            if (string.Equals(configTypeTemp, "live", StringComparison.InvariantCultureIgnoreCase)) {
                configType = ConfigurationType.Live;
            }

            var records = client.GetDataByVersion(configType, version, appType);

            var sb = new StringBuilder();
            sb.Append("/quote ");
            sb.Append("Org Code, Machine IPs, Live Version, QA Version, Web Server Name, Location\n");
            sb.Append("--------------------------------------------------------------------------\n");

            foreach (var record in records) {
                sb.AppendFormat(
                    "{0}, {1}, {2}, {3}, {4}, {5}\n",
                    record.OrgCode,
                    string.Join(",", record.MachineIPAddresses),
                    record.LiveVersion,
                    record.QAVersion,
                    record.WebServerNames,
                    record.Location);
            }

            sb.Append("--------------------------------------------------------------------------\n");
            sb.AppendFormat("Query: config={0} and version={1} and type={2}\n", configType, version, appType);
            sb.Append("Records Found: ");
            sb.Append(records.Count);
            sb.Append("\n--------------------------------------------------------------------------");

            return sb.ToString();
        }

        protected override string HelpDescription
        {
            get
            {
                return "";
            }
        }

        protected override string HelpExample
        {
            get
            {
                return "";
            }
        }

        protected override string HelpFormat
        {
            get
            {
                return "";
            }
        }
    }
}
