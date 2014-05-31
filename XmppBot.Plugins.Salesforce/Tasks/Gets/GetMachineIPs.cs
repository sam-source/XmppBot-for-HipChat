using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks.Gets
{
    class GetMachineIPs : SequenceTaskBase
    {
        public GetMachineIPs(string pluginName) : base(pluginName, "get-machineips") { }

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

            return Observable.Return(string.Format("{0}, the machine ips for {1} are {2}", user, args[0], record.TNEW_Machine_IPs__c));
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
            get { return "The command will retrieve the TNEW Machine IPs field from TND."; }
        }

        protected override string HelpExample
        {
            get { return "!tnd-get-machineips AAEM"; }
        }

        protected override string HelpFormat
        {
            get { return "This command requires one parameter, the TNEW Client Code. This value must mache a TNEW opportunity record in TND."; }
        }
    }
}
