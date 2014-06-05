using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using TND;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks.Sets
{
    public class SetVersion : SequenceTaskBase
    {
        public SetVersion(string pluginName) : base(pluginName, "set-version") { }

        protected override System.IObservable<string> ExecuteTask(ParsedLine taskInfo)
        {
            bool found = false;
            bool isLive = false;
            bool isQA = false;

            if (string.Equals(taskInfo.Args[1], "live", StringComparison.InvariantCultureIgnoreCase)) {
                found = true;
                isLive = true;
            }

            if (string.Equals(taskInfo.Args[1], "qa", StringComparison.InvariantCultureIgnoreCase)) {
                found = true;
                isQA = true;
            }

            if (!found) {
                return Observable.Return("I did not understand the second parameter. It must be 'qa' or 'live'.");
            }
            
            Version newVersion;

            if (!Version.TryParse(taskInfo.Args[2], out newVersion)) {
                return Observable.Return("That version number did not parse correctly.");
            }

            var provider = new TNEWProvider();
            var client = new Client();
            ParsedData record;

            try {
                record = provider.GetRecord(taskInfo.Args[0]);
            }
            catch (Exception ex) {
                if (ex.Message.Contains("Invalid Org Code")) {
                    return Observable.Return("That org code did not find any results.");
                }

                return Observable.Return("An unknown error occurred while fetching from TND. " + ex.Message);
            }

            if (record == null) {
                return Observable.Return("The client was not found in TND.");
            }

            if (isLive) {
                client.UpdateOpportunityVersion(record.Id, newVersion.ToString(2), record.QAVersion);
            }

            if (isQA) {
                client.UpdateOpportunityVersion(record.Id, record.LiveVersion, newVersion.ToString(2));
            }

            record = provider.GetRecord(taskInfo.Args[0]);

            return
                Observable.Return(
                    string.Format(
                        "{0}, here are {1}'s new versions from TND. Live: {2}  QA: {3} ",
                        taskInfo.NickName,
                        taskInfo.Args[0],
                        record.LiveVersion,
                        record.QAVersion));
        }

        protected override bool IsValid(ParsedLine taskInfo)
        {
            if (!this.IsMatch(taskInfo)) {
                return false;
            }

            if (taskInfo.Args.Count() != 3) {
                return false;
            }

            return true;
        }

        protected override string HelpDescription
        {
            get { return "This comman will update the version number in TND"; }
        }

        protected override string HelpExample
        {
            get { return "!tnd-set-version TWEB qa 5.5"; }
        }

        protected override string HelpFormat
        {
            get
            {
                return "This command has three parameters. The first parameter is the TNEW Client Code in TND. The second parameter is 'qa' or 'live' and will determine which version field in TND will get updated. The third parameter is the version number to save in the field. It must be a valid, parse-able version number.";
            }
        }
    }
}
