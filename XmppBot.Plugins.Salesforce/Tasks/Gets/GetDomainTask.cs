using System.Linq;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks
{
    public class GetDomainTask : SimpleTaskBase
    {
        public GetDomainTask(string pluginName) : base(pluginName, "get-domain") { }

        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            var user = taskInfo.NickName;
            var args = taskInfo.Args;
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
