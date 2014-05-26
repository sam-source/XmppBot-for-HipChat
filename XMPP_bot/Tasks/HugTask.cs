using System.Linq;

using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class HugTask : SimpleTaskBase
    {
        public HugTask() : base(null, "hug") { }

        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            return string.Format("/me hugs {0} (awthanks)", taskInfo.Args.FirstOrDefault() ?? "themself");
        }

        protected override string HelpDescription
        {
            get
            {
                return "This command will make the bot give a hug to another hipchat user.";
            }
        }

        protected override string HelpExample
        {
            get
            {
                return "!hug @smenard";
            }
        }

        protected override string HelpFormat
        {
            get
            {
                return "The command has one parameter. The parameter is the username of another hipchat user.";
            }
        }
    }
}
