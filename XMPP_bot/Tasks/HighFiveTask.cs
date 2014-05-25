using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class HighFiveTask : SimpleTaskBase
    {
        public override string Name
        {
            get
            {
                return "(highfive)";
            }
        }

        public override string Execute(ParsedLine taskInfo)
        {
            return "(highfive)";
        }
    }
}
