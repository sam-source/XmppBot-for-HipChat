using System.Linq;

using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class HugTask : SimpleTaskBase
    {
        public override string Name
        {
            get
            {
                return "hug";
            }
        }

        public override string Execute(ParsedLine taskInfo)
        {
            return string.Format("/me hugs {0} (awthanks)", taskInfo.Args.FirstOrDefault() ?? "themself");
        }
    }
}
