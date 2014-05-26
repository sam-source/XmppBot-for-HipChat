using System;
using System.Linq;
using System.Reactive.Linq;

using XmppBot.Common;

namespace XmppBot.Plugins.Deployments.Tasks.Gets
{
    class GetBuildStatusTask : SequenceTaskBase
    {
        public GetBuildStatusTask(string pluginName) : base(pluginName, "get-buildstatus") { }

        protected override IObservable<string> ExecuteTask(ParsedLine line)
        {
            var builder = new BambooConnection();

            var project = builder.GetPlanKey(line.Args);
            var buildNumber = line.Args.Last();

            return Observable.Return(builder.GetBuildState(project, buildNumber));
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
