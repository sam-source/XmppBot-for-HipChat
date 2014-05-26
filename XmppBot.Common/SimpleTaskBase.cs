namespace XmppBot.Common
{
    public abstract class SimpleTaskBase : BotTaskBase
    {
        public SimpleTaskBase(string pluginName, string commandName) : base(pluginName, commandName)
        {
        }

        public string Execute(ParsedLine taskInfo)
        {
            if (!this.IsMatch(taskInfo)) {
                return "Invalid command.\n" + this.GetHelp();
            }

            if (!this.IsValid(taskInfo)) {
                return "Invalid command arguments.\n" + this.GetHelp();
            }

            return this.ExecuteTask(taskInfo);
        }

        protected abstract string ExecuteTask(ParsedLine taskInfo);
    }
}
