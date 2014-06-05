using System;
using System.Linq;
using System.Reactive.Linq;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks
{
    public class GetDomainTask : SequenceTaskBase
    {
        public GetDomainTask(string pluginName) : base(pluginName, "get-domain") { }

        protected override IObservable<string> ExecuteTask(ParsedLine taskInfo)
        {
            var user = taskInfo.NickName;
            var args = taskInfo.Args;
            var client = new TND.Client();
            
            var records = client.GetTnewOpportunities(args[0], "TNEW_DNS_Subdomain__c");

            if (records == null) {
                return Observable.Return(user + ", that org code did not return any results");
            }

            var record = records.FirstOrDefault();

            if (record == null) {
                return Observable.Return(user + ", that org code did not return any results");
            }

            return
                Observable.Return(
                    string.Format(
                        "{0} the domain for {1} is {2}\nThe qa site is {3}",
                        user,
                        args[0],
                        record.TNEW_DNS_Subdomain__c,
                        record.TNEW_DNS_Subdomain__c.EndsWith("/")
                            ? record.TNEW_DNS_Subdomain__c + "_QA_"
                            : record.TNEW_DNS_Subdomain__c + "/_QA_"));
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
            get { return "The command will retrieve the url of the client's tnew website from TND."; }
        }

        protected override string HelpExample
        {
            get { return "!tnd-get-domain AAEM"; }
        }

        protected override string HelpFormat
        {
            get { return "This command requires one parameter, the TNEW Client Code. This value must mache a TNEW opportunity record in TND."; }
        }
    }
}
