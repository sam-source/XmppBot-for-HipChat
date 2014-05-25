using System;
using System.Linq;

using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class GiveTask : SimpleTaskBase
    {
        public override string Name
        {
            get
            {
                return "give";
            }
        }

        public override string Execute(ParsedLine taskInfo)
        {
            if (taskInfo.Args.Contains("dinosaur")) {
                return new Random(DateTime.Now.Second).Next(0, 5) >= 3 ? "(clevergirl)" : "(philosoraptor)";
            }

            if (taskInfo.Args.Contains("money")) {
                return ":$";
            }

            return "I have nothing to give, you took it all.";
        }
    }
}
