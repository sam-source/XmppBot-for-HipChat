using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class SmackTask : SimpleTaskBase
    {
        public SmackTask() : base(null, "smack") { }

        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            return string.Format("/me smacks {0} around with a large trout.", taskInfo.Args[0]);
        }

        protected override string HelpDescription
        {
            get
            {
                return "This command is used to alert another hipchat user if the user has notifications turned on.";
            }
        }

        protected override string HelpExample
        {
            get
            {
                return "!smack @smenard";
            }
        }

        protected override string HelpFormat
        {
            get
            {
                return "This command has one parameter. The parameter is the username of another hipchat user.";
            }
        }
    }
}
