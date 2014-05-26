using System;
using System.Reactive.Linq;

using XmppBot.Common;

namespace XmppBot.Plugins.Deployments.Tasks.Deploy
{
    class DeployContentTask : SequenceTaskBase
    {
        public DeployContentTask(string pluginName) : base(pluginName, "deploy-content") { }

        private static bool isDeploying = false;

        protected override System.IObservable<string> ExecuteTask(ParsedLine line)
        {
            var user = line.NickName;
            var project = "CB-TNEWRAMPQA";

            if (isDeploying) {
                return Observable.Return(string.Format("I can't do that {0}. There is a build already in progress.", user));
            }

            isDeploying = true;

            var builder = new BambooConnection();

            var buildNumber = builder.DeployContent(project, line.Args[1], line.Args[0]);

            int buildNumberInt;

            if (!int.TryParse(buildNumber, out buildNumberInt)) {
                return Observable.Return(buildNumber);
            }

            string buildState = null;
            int completedCount = 0;
            IObservable<string> seq =
                Observable.Interval(TimeSpan.FromSeconds(10))
                    .TakeWhile(l =>
                    {
                        buildState = builder.GetBuildState(project, buildNumber);

                        if (buildState == "Successful") {
                            isDeploying = false;
                            return false;
                        }

                        if (buildState.StartsWith("Failed")) {
                            isDeploying = false;
                            return false;
                        }

                        if (buildState.StartsWith("Build")) {
                            return true;
                        }

                        if (completedCount > 3) {
                            isDeploying = false;
                            return false;
                        }

                        completedCount++;
                        return true;
                    })
                    .Select(l => string.Format(buildState));

            return
                Observable.Return(string.Format("{0}, I have started the bamboo build. The build number is {1}", user, buildNumber))
                    .Concat(seq)
                    .Concat(Observable.Return(string.Format("Build {0} finished", buildNumber)));
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
