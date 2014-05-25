using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class GetNicknameTask : SimpleTaskBase
    {
        private NickNameProvider provider;

        public GetNicknameTask(NickNameProvider provider)
        {
            this.provider = provider;
        }

        public override string Name
        {
            get
            {
                return "get-nickname";
            }
        }

        public override string Execute(ParsedLine taskInfo)
        {
            if (!this.Match(taskInfo.Command)) {
                return "Invalid command.";
            }

            return this.provider.GetName(taskInfo.User);
        }
    }
}
