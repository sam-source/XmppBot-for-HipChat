using System.Linq;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks
{
    class GetVersion : SimpleTaskBase
    {
        public GetVersion(string pluginName)
            : base(pluginName, "get-version") { }

        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            var user = taskInfo.User;
            var args = taskInfo.Args;
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
