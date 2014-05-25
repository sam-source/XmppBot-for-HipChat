using System.Linq;

namespace XmppBot.Plugins.Salesforce
{
    internal class TNDGets
    {
        public string Domain(string user, string[] args)
        {
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

        public string Api(string user, string[] args)
        {
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

        public string Version(string user, string[] args)
        {
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
    }
}
