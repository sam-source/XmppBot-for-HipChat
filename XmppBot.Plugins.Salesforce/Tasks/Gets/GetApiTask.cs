using System.Linq;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks
{
    class GetApiTask : SimpleTaskBase
    {
        public GetApiTask(string pluginName) : base(pluginName, "get-api") { }

        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            var user = taskInfo.NickName;
            var args = taskInfo.Args;
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
