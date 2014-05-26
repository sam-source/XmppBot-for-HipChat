using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class CloseTask : SimpleTaskBase
    {
        public CloseTask() : base(null, "close") { }

        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            System.Environment.Exit(0);

            return null;
        }

        protected override string HelpDescription
        {
            get
            {
                return "This command will force the service that runs the bot to shutdown.";
            }
        }

        protected override string HelpExample
        {
            get
            {
                return "!close";
            }
        }

        protected override string HelpFormat
        {
            get
            {
                return "The command does not have any parameters.";
            }
        }
    }
}
