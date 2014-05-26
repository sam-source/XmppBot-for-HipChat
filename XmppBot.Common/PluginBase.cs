using System;
using System.Collections.Generic;
using System.Linq;

namespace XmppBot.Common
{
    public abstract class PluginBase<TTask>
        where TTask : BotTaskBase
    {
        protected List<TTask> Tasks;

        public string Name { get; private set; }

        public PluginBase(string pluginName)
        {
            this.Name = pluginName;
            this.Tasks = new List<TTask>();
        }

        public IEnumerable<string> TaskKeys
        {
            get
            {
                return this.Tasks.Select(t => t.FullName).ToList();
            }
        }

        public BotTaskBase GetTask(string fullTaskName)
        {
            return this.Tasks.FirstOrDefault(t => string.Equals(t.FullName, fullTaskName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
