using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class CloseTask : SimpleTaskBase
    {
        public override string Name
        {
            get
            {
                return "close";
            }
        }

        public override string Execute(ParsedLine taskInfo)
        {
            if (!this.Match(taskInfo.Command)) {
                return "Invalid command.";
            }

            System.Environment.Exit(0);

            return null;
        }
    }
}
