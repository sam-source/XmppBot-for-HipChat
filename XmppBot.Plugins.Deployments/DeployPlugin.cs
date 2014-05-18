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
    public class DeployPlugin : IXmppBotSequencePlugin
    {
        public IObservable<string> Evaluate(ParsedLine line)
        {
            if (!line.IsCommand || !line.Command.ToLower().StartsWith("build")) {
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
                }
            }

            return Observable.Return(help);
        }

        public IObservable<string> CheckBuildStatus(ParsedLine line, string user)
        {
            var builder = new BambooConnection();

            var project = builder.GetPlanKey(line.Args);
            var buildNumber = line.Args.Last();

            return Observable.Return(builder.GetBuildState(project, buildNumber));
        }

        public IObservable<string> StartBuild(ParsedLine line, string user)
        {
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
                            else {
                                if (completedCount > 0) {
                                    return false;
                                }
                                completedCount++;
                                return true;
                            }
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
                return "Deployer";
            }
        }
    }
}
