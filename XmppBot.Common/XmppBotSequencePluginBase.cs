using System;
using System.Linq;
using System.Reactive.Linq;

namespace XmppBot.Common
{
    public abstract class XmppBotSequencePluginBase : PluginBase<SequenceTaskBase>
    {
        public XmppBotSequencePluginBase(string pluginName) : base(pluginName)
        {
        }

        public IObservable<string> ExecuteTask(ParsedLine line)
        {
            var task = this.Tasks.FirstOrDefault(t => t.IsMatch(line));

            if (task == null) {
                return Observable.Return("Command not found.");
            }

            return task.Execute(line);
        }
    }
}