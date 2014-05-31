using System;
using System.Text;

namespace XmppBot.Common
{
    public abstract class BotTaskBase
    {
        protected string PluginName { get; private set; }

        protected string CommandName { get; private set; }

        public string FullName { get; private set; }

        public BotTaskBase(string pluginName, string commandName)
        {
            this.PluginName = pluginName;
            this.CommandName = commandName;

            this.FullName = this.BuildCommandName(pluginName, commandName);
        }

        private string BuildCommandName(string pluginName, string commandName)
        {
            if (string.IsNullOrWhiteSpace(pluginName)) {
                return commandName;
            }

            return string.Format("{0}-{1}", pluginName, commandName);
        }

        protected virtual bool IsValid(ParsedLine taskInfo)
        {
            if (this.IsMatch(taskInfo)) {
                return true;
            }

            return false;
        }

        public bool IsMatch(ParsedLine taskInfo)
        {
            return string.Equals(taskInfo.Command, this.FullName, StringComparison.InvariantCultureIgnoreCase);
        }
        public string GetHelp()
        {
            var sb = new StringBuilder();

            sb.Append("/quote Command: ");
            sb.Append(this.FullName);
            sb.Append("\nDescription: ");
            sb.Append(this.HelpDescription);
            sb.Append("\nFormat: ");
            sb.Append(this.HelpFormat);
            sb.Append("\nExample: ");
            sb.Append(this.HelpExample);

            return sb.ToString();
        }
        protected abstract string HelpDescription { get; }

        protected abstract string HelpExample { get; }

        protected abstract string HelpFormat { get; }
    }
}
