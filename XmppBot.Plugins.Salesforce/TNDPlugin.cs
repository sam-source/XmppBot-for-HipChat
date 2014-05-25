using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

using XmppBot.Common;

namespace XmppBot.Plugins.Salesforce
{
    [Export(typeof(IXmppBotPlugin))]
    public class TNDPlugin : IXmppBotPlugin
    {
        public TNDPlugin()
        {
            var temp = new List<string>();

            temp.Add("get-domain");
            temp.Add("get-version");
            temp.Add("get-api");

            this.TaskKeys = temp;
        }

        public IEnumerable<string> TaskKeys { get; private set; }

        public string ExecuteTask(ParsedLine line)
        {
            if (!line.IsCommand) {
                return null;
            }

            var pieces = line.Command.Split('-');

            if (pieces.Length != 3) {
                return null;
            }

            if (string.Equals(pieces[1], "get", StringComparison.InvariantCultureIgnoreCase)) {
                return this.EvaluateGet(pieces[2], line.NickName, line.Args);
            }

            return null;
        }

        private string EvaluateGet(string target, string user, string[] args)
        {
            if (args == null || args.Length == 0) {
                return null;
            }

            var retrieve = new TNDGets();

            switch (target) {
                case "domain":
                    return retrieve.Domain(user, args);
                case "api":
                    return retrieve.Api(user, args);
                case "version":
                    return retrieve.Version(user, args);
            }

            return null;
        }

        public string Name
        {
            get
            {
                return "tnd";
            }
        }
    }
}
