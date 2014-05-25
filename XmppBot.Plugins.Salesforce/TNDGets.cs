using System;
using System.Linq;
using System.Text;

using TND;

namespace XmppBot.Plugins.Salesforce
{
    internal class TNDGets
    {
        public string Domain(string user, string[] args)
        {
            var client = new TND.Client();
            var records = client.GetTnewOpportunities(args[0], "TNEW_DNS_Subdomain__c");

            if (records == null) {
                return user + ", that org code did not return any results";
            }

            var record = records.FirstOrDefault();

            if (record == null) {
                return user + ", that org code did not return any results";
            }

            return string.Format("{0} the domain for {1} is {2}", user, args[0], record.TNEW_DNS_Subdomain__c);
        }

        public string Api(string user, string[] args)
        {
            var client = new TND.Client();
            var records = client.GetTnewOpportunities(args[0], "TNEW_Test_API_URL__c", "TNEW_Live_API_URL__c");

            if (records == null) {
                return user + ", that org code did not return any results";
            }

            var record = records.FirstOrDefault();

            if (record == null) {
                return user + ", that org code did not return any results";
            }
            
            return string.Format("{0} the live api for {1} is {2} and the test api is {3}", user, args[0], record.TNEW_Live_API_URL__c, record.TNEW_Test_API_URL__c);
        }

        public string Version(string user, string[] args)
        {
            var client = new TND.Client();
            var records = client.GetTnewOpportunities(args[0], "TNEW_Version_Live__c", "TNEW_Version_QA__c");

            if (records == null) {
                return user + ", that org code did not return any results";
            }

            var record = records.FirstOrDefault();

            if (record == null) {
                return user + ", that org code did not return any results";
            }

            return string.Format("{0} the live version for {1} is {2} and the test version is {3}", user, args[0], record.TNEW_Version_Live__c, record.TNEW_Version_QA__c);
        }

        public string Query (string user, string[]  args)
        {
            //tnd-get-query type=tnew version=4.5 location=ramp config=live

            var client = new TND.TNEWProvider();

            var type = args.FirstOrDefault(a => a.StartsWith("type"));
            
            ApplicationType appType = ApplicationType.TNEW;

            if (!string.IsNullOrWhiteSpace(type)) {
                if (string.Equals(type, "type=tnst", StringComparison.InvariantCultureIgnoreCase)) {
                    appType = ApplicationType.TNST;
                }
            }

            type = args.FirstOrDefault(a => a.StartsWith("version"));

            if(string.IsNullOrWhiteSpace(type)) {
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
    }
}
