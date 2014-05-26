using System;
using System.Reactive.Linq;
using System.Text;

namespace XmppBot.Common
{
    public abstract class SequenceTaskBase : BotTaskBase
    {
        public SequenceTaskBase(string pluginName, string commandName) : base(pluginName, commandName)
        {
        }

        public IObservable<string> Execute(ParsedLine taskInfo)
        {
            if (!this.IsMatch(taskInfo)) {
                return Observable.Return("Invalid command.\n" + this.GetHelp());
            }

            if (!this.IsValid(taskInfo)) {
                return Observable.Return("Invalid command arguments.\n" + this.GetHelp());
            }

            return this.ExecuteTask(taskInfo);
        }

        protected abstract IObservable<string> ExecuteTask(ParsedLine taskInfo);
    }
}
