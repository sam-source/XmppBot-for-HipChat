using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks.Fixes
{
    class FixMachineIPsTask : SequenceTaskBase
    {
        public FixMachineIPsTask(string pluginName) : base(pluginName, "fix-machineips") { }

        protected override IObservable<string> ExecuteTask(ParsedLine taskInfo)
        {
            var user = taskInfo.NickName;
            var args = taskInfo.Args;
            var client = new TND.Client();
            var records = client.GetTnewOpportunities(args[0], "TNEW_Machine_IPs__c");

            if (records == null || records.Count == 0) {
                return Observable.Return(string.Format("{0} the record was not found for org code: {1}", user, args[1]));
            }

            var record = records.First();

            var pieces = record.TNEW_Machine_IPs__c.Split(new[] { ',', '/' });

            if (pieces.Length != 2 || pieces[1].Contains(".") || pieces[0].Count(c => c == '.') != 3) {
                return Observable.Return(string.Format("{0} I could not automatically parse the machine ips for org code: {1}", user, args[0]));
            }

            int lastOctet;

            if (!int.TryParse(pieces[1], out lastOctet)) {
                return Observable.Return(string.Format("{0} I could not automatically parse the machine ips for org code: {1}", user, args[0]));
            }

            var firstThreeOctets = pieces[0].Substring(0, pieces[0].LastIndexOf('.') + 1);

            var secondIP = firstThreeOctets + pieces[1];

            client.UpdateOpportunityMachineIPs(args[0], pieces[0], secondIP);

            var tnewClient = new TND.TNEWProvider();

            var op = tnewClient.GetRecord(args[0]);

            return Observable.Return(string.Format("{0} the record was saved. {1}", user, string.Join(",", op.MachineIPAddresses)));
        }

        protected virtual bool IsValid(ParsedLine taskInfo)
        {
            if (this.IsMatch(taskInfo)) {
                return true;
            }

            if (taskInfo.Args.Count() == 1) {
                return true;
            }

            return false;
        }

        protected override string HelpDescription
        {
            get { return "The command will read the Machine IPs field from TND and convert into fully formed ip addresses. For example it will change 10.0.0.1/2 to 10.0.0.1,10.0.0.2."; }
        }

        protected override string HelpExample
        {
            get { return "!tnd-fix-machineips AAEM"; }
        }

        protected override string HelpFormat
        {
            get { return "This command requires one parameter, the TNEW Client Code. This value must mache a TNEW opportunity record in TND."; }
        }
    }
}
