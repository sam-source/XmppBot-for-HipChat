using System;
using System.Linq;
using System.Reactive.Linq;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks.Sets
{
    class SetMachineIPsTask : SequenceTaskBase
    {
        public SetMachineIPsTask(string pluginName) : base(pluginName, "set-machineips") { }

        protected override IObservable<string> ExecuteTask(ParsedLine taskInfo)
        {
            var args = taskInfo.Args;
            var user = taskInfo.NickName;

            var client = new TND.Client();

            client.UpdateOpportunityMachineIPs(args[0], args[1], args[2]);

            var tnewClient = new TND.TNEWProvider();

            var op = tnewClient.GetRecord(args[0]);

            return Observable.Return(string.Format("{0} the record was saved. {1}", user, string.Join(",", op.MachineIPAddresses)));
        }

        protected virtual bool IsValid(ParsedLine taskInfo)
        {
            if (this.IsMatch(taskInfo)) {
                return true;
            }

            if (taskInfo.Args.Count() == 3) {
                return true;
            }

            return false;
        }

        protected override string HelpDescription
        {
            get { return "The command will update the Machine IPs field in TND for the organization specified."; }
        }

        protected override string HelpExample
        {
            get { return "!tnd-set-machineips AAEM 10.0.0.1 10.0.0.2"; }
        }

        protected override string HelpFormat
        {
            get { return "This command requires three parameters. The first parameter is the TNEW Client Code. The second and third parameters are ip addresses that will be concatenated into a comma separated list upon saving to TND."; }
        }
    }
}
