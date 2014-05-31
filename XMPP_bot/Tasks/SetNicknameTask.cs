using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class SetNicknameTask : SimpleTaskBase
    {
        private NickNameProvider provider;

        public SetNicknameTask(NickNameProvider provider)
            : base(null, "set-nickname")
        {
            this.provider = provider;
        }

        protected override bool IsValid(ParsedLine taskInfo)
        {
            if (this.IsMissingArgs(taskInfo)) {
                return false;
            }

            return true;
        }

        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            var nickname = string.Join(" ", taskInfo.Args);

            this.provider.SaveName(taskInfo.User, nickname);

            return string.Format("Okay {0}, I will call you {1}", taskInfo.User, nickname);
        }

        protected override string HelpDescription
        {
            get
            {
                return "This command allows you to assign a nickname to your user. The bot will replace your hipchat name with the nickname when addressing you.";
            }
        }

        protected override string HelpExample
        {
            get
            {
                return "!set-nickname Number One";
            }
        }

        protected override string HelpFormat
        {
            get
            {
                return "A nickname can be one or more names. The names are delimited with the space character.";
            }
        }

        private bool IsMissingArgs(ParsedLine taskInfo)
        {
            return taskInfo.Args == null || taskInfo.Args.Length == 0;
        }
    }
}
