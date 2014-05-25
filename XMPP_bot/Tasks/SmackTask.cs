using System;

using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class SmackTask : SimpleTaskBase
    {
        public override string Name
        {
            get
            {
                return "smack";
            }
        }

        public override string Execute(ParsedLine taskInfo)
        {
            return string.Format("/me smacks {0} around with a large trout.", taskInfo.Args[0]);
        }
    }
}
