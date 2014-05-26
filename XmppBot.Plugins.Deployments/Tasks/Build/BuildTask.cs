using System;
using System.Reactive.Linq;

using XmppBot.Common;

namespace XmppBot.Plugins.Deployments.Tasks.Build
{
    class BuildTask : SequenceTaskBase
    {
        private static bool isBuilding = false;

        public BuildTask(string pluginName) : base(pluginName, "build") { }

        protected override IObservable<string> ExecuteTask(ParsedLine line)
        {
            var user = line.NickName;

            if (isBuilding) {
                return Observable.Return(string.Format("I can't do that {0}. There is a build already in progress.", user));
            }

            isBuilding = true;

            var builder = new BambooConnection();

            var project = builder.GetPlanKey(line.Args);

            var buildNumber = builder.StartBuild(line.Args);

            int buildNumberInt;

            if (!int.TryParse(buildNumber, out buildNumberInt)) {
                return Observable.Return(buildNumber);
            }

            string buildState = null;
            int completedCount = 0;
            IObservable<string> seq =
                Observable.Interval(TimeSpan.FromSeconds(30))
                    .TakeWhile(l =>
                    {
                        buildState = builder.GetBuildState(project, buildNumber);

                        if (buildState.StartsWith("Build")) {
                            return true;
                        }
                        if (buildState.StartsWith("Failed")) {
                            isBuilding = false;
                            return false;
                        }

                        if (completedCount > 0) {
                            isBuilding = false;
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
