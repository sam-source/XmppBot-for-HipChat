using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using TND;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks
{
    class GetQueryTask : SequenceTaskBase
    {
        public GetQueryTask(string pluginName)
            : base(pluginName, "get-query") { }

        protected override IObservable<string> ExecuteTask(ParsedLine taskInfo)
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
                return Observable.Return("Filter 'version' is required. Example: 'version=4.5'");
            }

            System.Version version = new System.Version(type.Substring(type.IndexOf("=") + 1));

            type = args.FirstOrDefault(a => a.StartsWith("config"));

            if (string.IsNullOrWhiteSpace(type)) {
                return Observable.Return("Filter 'config' is required. Example: 'config=live'");
            }

            TND.ConfigurationType configType = ConfigurationType.QA;

            var configTypeTemp = type.Substring(type.IndexOf("=") + 1);

            if (string.Equals(configTypeTemp, "live", StringComparison.InvariantCultureIgnoreCase)) {
                configType = ConfigurationType.Live;
            }

            type = args.FirstOrDefault(a => a.StartsWith("location"));

            TND.DomainLocation location = DomainLocation.NorthAmerica;

            if (string.Equals(type, "location=aws", StringComparison.InvariantCultureIgnoreCase)) {
                location = DomainLocation.Australia;
            }

            var records = client.GetDataByVersion(configType, version, appType, location);

            var sb = new StringBuilder();
            sb.Append("/quote ");
            sb.Append("Org Code, Machine IPs, Live Version, QA Version, Web Server Name, Location, domain\n");
            sb.Append("--------------------------------------------------------------------------\n");

            foreach (var record in records) {
                sb.AppendFormat(
                    "{0}, {1}, {2}, {3}, {4}, {5}, {6}\n",
                    record.OrgCode,
                    string.Join(",", record.MachineIPAddresses),
                    record.LiveVersion,
                    record.QAVersion,
                    record.WebServerNames,
                    record.Location,
                    record.Domain);
            }

            sb.Append("--------------------------------------------------------------------------\n");
            sb.AppendFormat("Query: config={0} and version={1} and type={2} and location={3}\n", configType, version, appType, location);
            sb.Append("Records Found: ");
            sb.Append(records.Count);
            sb.Append("\n--------------------------------------------------------------------------");

            return Observable.Return(sb.ToString());
        }

        protected virtual bool IsValid(ParsedLine taskInfo)
        {
            if (this.IsMatch(taskInfo)) {
                return true;
            }

            if (taskInfo.Args.Count() > 1) {
                return true;
            }

            return false;
        }

        protected override string HelpDescription
        {
            get { return "The command will retrieve a list of clients from TND that match the query parameters."; }
        }

        protected override string HelpExample
        {
            get { return "!tnd-get-query config=qa version=4.5 type=tnew location=aws"; }
        }

        protected override string HelpFormat
        {
            get { return "This command has two required parameters and one optional parameter. The two required parameters are config={?} and version={?}. The config parameter value can be 'live' or 'qa'. The version parameter needs to match the value stored within TND. The option parameter is type={?}. Type can have a value of 'tnew' or 'tnst'."; }
        }
    }
}
