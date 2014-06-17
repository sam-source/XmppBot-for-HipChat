using System;
using System.Reactive.Linq;

using TND;
using TND.SforceApi;

using XmppBot.Common;

namespace XmppBot.Plugins.Deployments.Tasks.Deploy
{
    class DeployContentTask : SequenceTaskBase
    {
        public DeployContentTask(string pluginName) : base(pluginName, "deploy-content") { }

        private static bool isDeploying = false;

        protected override IObservable<string> ExecuteTask(ParsedLine line)
        {
            var user = line.NickName;

            if (isDeploying) {
                return Observable.Return(string.Format("I can't do that {0}. There is a deploy already in progress.", user));
            }
            
            /*
             curl -X POST --user eve:Tr1fl3! "http://bamboo.tessituranetwork.com:8085/rest/api/latest/queue/CB-TNEWRAMPQA?os_authType=basic&bamboo.variable.orgCodes=TWEB&bamboo.variable.targetEnvironment=QA"
             */

            var orgs = line.Args[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string buildKey;
            ParsedData record = null;
            
            DomainLocation lastLocation = DomainLocation.Unknown;
            string lastOrg = "";

            foreach (var org in orgs) {
                var provider = new TNEWProvider();
                record = provider.GetRecord(org);

                if (lastLocation == DomainLocation.Unknown) {
                    lastLocation = record.Location;
                    lastOrg = org;
                }
                else {
                    if (lastLocation != record.Location) {
                        Observable.Return(
                            string.Format(
                                "TND shows {0}'s location as {1} and {2}'s location as {3}. Locations must match.",
                                record.OrgCode,
                                record.Location,
                                lastOrg,
                                lastLocation));
                    }
                }

                Version deployVerion;

                if (string.Equals(line.Args[0], "live", StringComparison.InvariantCultureIgnoreCase)) {
                    if (string.IsNullOrWhiteSpace(record.LiveVersion)) {
                        return Observable.Return(string.Format("{0}, {1} does not have a live version number set. Use the tnd-set-version command to set their version number.", user, org));
                    }

                    deployVerion = Version.Parse(record.LiveVersion);
                }
                else {
                    if (string.IsNullOrWhiteSpace(record.QAVersion)) {
                        return Observable.Return(string.Format("{0}, {1} does not have a qa version number set. Use the tnd-set-version command to set their version number.", user, org));
                    }

                    deployVerion = Version.Parse(record.QAVersion);
                }

                if (deployVerion < new Version(5, 0)) {
                    return
                        Observable.Return(
                            string.Format(
                                "TND shows {2}'s {0} version as {1}. To run this deployment, they must be version 5.0 or greater.",
                                line.Args[0],
                                deployVerion,
                                org));
                }
            }

            switch (record.Location) {
                case DomainLocation.NorthAmerica:
                    buildKey = "CB-TNEWRAMPQA";
                    break;
                  case DomainLocation.Australia:
                    buildKey = "CB-TNAWS";
                    break;
                default:
                    return Observable.Return("Location of client could not be determined.");
            }

            var builder = new BambooConnection();

            var buildNumber = builder.DeployContent(buildKey, line.Args[1], line.Args[0]);

            int buildNumberInt;

            if (!int.TryParse(buildNumber, out buildNumberInt)) {
                return Observable.Return(buildNumber);
            }
            
            isDeploying = true;

            string buildState = null;
            int completedCount = 0;
            IObservable<string> seq =
                Observable.Interval(TimeSpan.FromSeconds(10))
                    .TakeWhile(l =>
                    {
                        buildState = builder.GetBuildState(buildKey, buildNumber);

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
                Observable.Return(string.Format("{0}, I have started the bamboo build for {2}. The build number is {1}", user, buildNumber, record.Name))
                    .Concat(seq)
                    .Concat(Observable.Return(string.Format("Build {0} finished", buildNumber)));
        }

        protected override string HelpDescription
        {
            get { return "This command will deploy content to a client's website. This command will query TND to determine if the client is in the RAMP environment or the Australian environment."; }
        }

        protected override string HelpExample
        {
            get { return "!bamboo-deploy-content qa TWEB"; }
        }

        protected override string HelpFormat
        {
            get { return "Two parameters are required for this command. The first parameter is 'qa' or 'live'. The second parameter is the organization code."; }
        }
    }
}
