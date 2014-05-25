using System;

namespace XmppBot.Common
{
    public abstract class SimpleTaskBase
    {
        public abstract string Name { get; }

        public abstract string Execute(ParsedLine taskInfo);

        public bool Match(string commandName)
        {
            return string.Equals(commandName.Trim(), this.Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
