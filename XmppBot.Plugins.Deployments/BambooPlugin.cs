using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using XmppBot.Common;

namespace XmppBot.Plugins.Deployments
{
    [System.ComponentModel.Composition.Export(typeof(IXmppBotSequencePlugin))]
    public class BambooPlugin : IXmppBotSequencePlugin
    {
        public BambooPlugin()
        {
            var temp = new List<string>();

            temp.Add("build");
            temp.Add("get-buildstatus");
            temp.Add("deploy-content");

            this.TaskKeys = temp;
        }

        public IEnumerable<string> TaskKeys { get; private set; }

        public IObservable<string> ExecuteTask(ParsedLine line)
        {
            if (!line.IsCommand || (!line.Command.ToLower().StartsWith("build") && !line.Command.ToLower().StartsWith("bamboo"))) {
                return null;
            }

            string help = "@bot build [product] [qa or live] [option: aws]";

            string user = line.User == "Samuel Menard" ? "Sam" : line.User;

            if (line.IsCommand) {
                switch (line.Command.ToLower()) {
                    case "build":
                        return this.StartBuild(line, user);
                    case "build-status":
                        return this.CheckBuildStatus(line, user);
                    case "bamboo-deploy-content":
                        return this.DeployContent(line, user);
                }
            }

            return Observable.Return(help);
        }

        private static bool isDeploying = false;

        private IObservable<string> DeployContent(ParsedLine line, string user)
        {
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

        public IObservable<string> CheckBuildStatus(ParsedLine line, string user)
        {
            var builder = new BambooConnection();

            var project = builder.GetPlanKey(line.Args);
            var buildNumber = line.Args.Last();

            return Observable.Return(builder.GetBuildState(project, buildNumber));
        }

        private static bool isBuilding = false;

        public IObservable<string> StartBuild(ParsedLine line, string user)
        {
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

        public string Name
        {
            get
            {
                return "bamboo";
            }
        }
    }
}
