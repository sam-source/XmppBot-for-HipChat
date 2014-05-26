using System.Linq;

namespace XmppBot.Common
{
    public abstract class XmppBotPluginBase : PluginBase<SimpleTaskBase>
    {
        public XmppBotPluginBase(string pluginName) : base(pluginName)
        {
        }

        public string ExecuteTask(ParsedLine line)
        {
            var task = this.Tasks.FirstOrDefault(t => t.IsMatch(line));

            if (task == null) {
                return "Command not found.";
            }

            return task.Execute(line);
        }
    }
}
