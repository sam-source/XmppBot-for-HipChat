using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using TND;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce.Tasks.Ping
{
    class PingVersion : SequenceTaskBase
    {
        public PingVersion(string pluginName)
            : base(pluginName, "ping-version")
        {
            
        }

        protected override IObservable<string> ExecuteTask(ParsedLine taskInfo)
        {
            return Observable.Create<string>(
                observer =>
                {
                    observer.OnNext(string.Format("{0}, I'm retrieving the site list from tnd for TNEW version {1}", taskInfo.NickName, taskInfo.Args[0]));
                    
                    var failedCounter = 0;

                    var provider = new TNEWProvider();

                    var records = provider.GetDataByVersion(ConfigurationType.Live, Version.Parse(taskInfo.Args[0]));

                    observer.OnNext(string.Format("{0} I found {1} sites for version {2} of TNEW", taskInfo.NickName, records.Count, taskInfo.Args[0]));

                    records.AsParallel().ForAll(
                        record =>
                        {
                            try {
                                var url = record.Domain.StartsWith("http") ? record.Domain : "http://" + record.Domain;

                                var client = new WebClient();

                                var temp = client.DownloadString(url);
                                observer.OnNext(
                                    string.Format("{0} ({2}) page size {1}\n", record.OrgCode, temp.Length, url));
                            }
                            catch (Exception ex) {
                                observer.OnNext(string.Format("{0} failed : {1}\n", record.OrgCode, ex.Message));
                                failedCounter++;
                            }
                        });

                    observer.OnNext(
                        string.Format(
                            "{0}, I have completed querying all of the websites. There {1}",
                            taskInfo.NickName,
                            failedCounter == 0
                                ? "were no failures."
                                : failedCounter == 1
                                    ? "was one failure"
                                    : string.Format("were {0} failures", failedCounter)));
                    return Disposable.Empty;
                });
        }

        protected override bool IsValid(ParsedLine taskInfo)
        {
            if (!this.IsMatch(taskInfo)) {
                return false;
            }

            if (taskInfo.Args == null || taskInfo.Args.Count() != 1) {
                return false;
            }

            return true;
        }

        protected override string HelpDescription
        {
            get { return "This will ping each domain to verify their landing page is accessible."; }
        }

        protected override string HelpExample
        {
            get { return "!tnd-ping-version 3.5"; }
        }

        protected override string HelpFormat
        {
            get { return ""; }
        }
    }
}
