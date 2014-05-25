using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class SetNicknameTask : SimpleTaskBase
    {
        private NickNameProvider provider;

        public SetNicknameTask(NickNameProvider provider)
        {
            this.provider = provider;
        }

        public override string Name
        {
            get
            {
                return "set-nickname";
            }
        }

        public override string Execute(ParsedLine taskInfo)
        {
            if (this.IsMissingArgs(taskInfo)) {
                return this.GetHelpString();
            }

            if (!this.Match(taskInfo.Command)) {
                return "Invalid command.";
            }

            var nickname = string.Join(" ", taskInfo.Args);

            this.provider.SaveName(taskInfo.User, nickname);

            return string.Format("Okay {0}, I will call you {1}", taskInfo.User, nickname);
        }

        private string GetHelpString()
        {
            return "Invalid Command.\nCommand format: set-nickname [name]( [name])";
        }

        private bool IsMissingArgs(ParsedLine taskInfo)
        {
            return taskInfo.Args == null || taskInfo.Args.Length == 0;
        }
    }
}
