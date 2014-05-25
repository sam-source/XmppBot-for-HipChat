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
            temp.Add("get-query");
            temp.Add("set-machineips");
            temp.Add("fix-machineips");

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

            if (string.Equals(pieces[1], "set", StringComparison.InvariantCultureIgnoreCase)) {
                return this.EvaluateSet(pieces[2], line.NickName, line.Args);
            }

            if (string.Equals(pieces[1], "fix", StringComparison.InvariantCultureIgnoreCase)) {
                return this.EvaluateFix(pieces[2], line.NickName, line.Args);
            }

            return null;
        }

        private string EvaluateFix(string target, string user, string[] args)
        {
            if (args == null || args.Length == 0) {
                return null;
            }

            var fixer = new TNDFixes();

            switch (target) {
                case "machineips":
                    return fixer.MachineIPs(user, args);
            }

            return null;
        }

        private string EvaluateSet(string target, string user, string[] args)
        {
            if (args == null || args.Length == 0) {
                return null;
            }

            var saver = new TNDSets();

            switch (target) {
                case "machineips":
                    return saver.MachineIPs(user, args);
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
                case "query":
                    return retrieve.Query(user, args);
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
