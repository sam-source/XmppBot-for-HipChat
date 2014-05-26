using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class GetNicknameTask : SimpleTaskBase
    {
        private NickNameProvider provider;

        public GetNicknameTask(NickNameProvider provider) : base(null, "get-nickname")
        {
            this.provider = provider;
        }

        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            return this.provider.GetName(taskInfo.User);
        }

        protected override string HelpDescription
        {
            get
            {
                return "This command will retrieve the currently set nickname for your user.";
            }
        }

        protected override string HelpExample
        {
            get
            {
                return "!" + this.FullName;
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
