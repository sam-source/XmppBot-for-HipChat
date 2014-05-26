using System.Text;

using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class HighFiveTask : SimpleTaskBase
    {
        public HighFiveTask() : base(null, "(highfive)")
        {
        }

        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            return "(highfive)";
        }

        protected override string HelpDescription
        {
            get
            {
                return "This command will have the bot return a highfive when directed towards the bot.";
            }
        }

        protected override string HelpExample
        {
            get
            {
                return "(highfive)";
            }
        }

        protected override string HelpFormat
        {
            get
            {
                return "( highfive) without the space.";
            }
        }
    }
}
