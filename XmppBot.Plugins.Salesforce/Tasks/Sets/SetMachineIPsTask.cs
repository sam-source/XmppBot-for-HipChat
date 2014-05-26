using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks.Sets
{
    class SetMachineIPsTask : SimpleTaskBase
    {
        public SetMachineIPsTask(string pluginName) : base(pluginName, "set-machineips") { }

        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            var args = taskInfo.Args;
            var user = taskInfo.NickName;

            var client = new TND.Client();

            client.UpdateOpportunityMachineIPs(args[0], args[1], args[2]);

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
