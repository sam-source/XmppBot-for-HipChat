using System.Linq;
using System.Threading.Tasks;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks.Fixes
{
    class FixMachineIPsTask : SimpleTaskBase
    {
        public FixMachineIPsTask(string pluginName) : base(pluginName, "fix-machineips") { }

        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            var user = taskInfo.NickName;
            var args = taskInfo.Args;
            var client = new TND.Client();
            var records = client.GetTnewOpportunities(args[0], "TNEW_Machine_IPs__c");

            if (records == null || records.Count == 0) {
                return string.Format("{0} the record was not found for org code: {1}", user, args[1]);
            }

            var record = records.First();

            var pieces = record.TNEW_Machine_IPs__c.Split(new[] { ',', '/' });

            if (pieces.Length != 2 || pieces[1].Contains(".") || pieces[0].Count(c => c == '.') != 3) {
                return string.Format("{0} I could not automatically parse the machine ips for org code: {1}", user, args[0]);
            }

            int lastOctet;

            if (!int.TryParse(pieces[1], out lastOctet)) {
                return string.Format("{0} I could not automatically parse the machine ips for org code: {1}", user, args[0]);
            }

            var firstThreeOctets = pieces[0].Substring(0, pieces[0].LastIndexOf('.') + 1);

            var secondIP = firstThreeOctets + pieces[1];

            client.UpdateOpportunityMachineIPs(args[0], pieces[0], secondIP);

            var tnewClient = new TND.TNEWProvider();

            var op = tnewClient.GetRecord(args[0]);

            return string.Format("{0} the record was saved. {1}", user, string.Join(",", op.MachineIPAddresses));
        }

        protected override string HelpDescription
        {
            get { return ""; }
        }

        protected override string HelpExample
        {
            get { return ""; }
        }

        protected override string HelpFormat
        {
            get { return ""; }
        }
    }
}
