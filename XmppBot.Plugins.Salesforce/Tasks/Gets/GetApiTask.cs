using System;
using System.Linq;
using System.Reactive.Linq;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks
{
    class GetApiTask : SequenceTaskBase
    {
        public GetApiTask(string pluginName) : base(pluginName, "get-api") { }

        protected override IObservable<string> ExecuteTask(ParsedLine taskInfo)
        {
            var user = taskInfo.NickName;
            var args = taskInfo.Args;
            var client = new TND.Client();
            var records = client.GetTnewOpportunities(args[0], "TNEW_Test_API_URL__c", "TNEW_Live_API_URL__c");

            if (records == null) {
                return Observable.Return(user + ", that org code did not return any results");
            }

            var record = records.FirstOrDefault();

            if (record == null) {
                return Observable.Return(user + ", that org code did not return any results");
            }

            return Observable.Return(string.Format("{0} the live api for {1} is {2} and the test api is {3}", user, args[0], record.TNEW_Live_API_URL__c, record.TNEW_Test_API_URL__c));
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
            get { return "The command will retrieve the TNEW Test API Url and TNEW Live API Urls from TND."; }
        }

        protected override string HelpExample
        {
            get { return "!tnd-get-api AAEM"; }
        }

        protected override string HelpFormat
        {
            get { return "This command requires one parameter, the TNEW Client Code. This value must mache a TNEW opportunity record in TND."; }
        }
    }
}
