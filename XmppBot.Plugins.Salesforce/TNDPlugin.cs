using System.Collections.Generic;
using System.ComponentModel.Composition;

using XmppBot.Common;
using XmppBot.Plugins.Salesforce.Tasks;
using XmppBot.Plugins.Salesforce.Tasks.Fixes;
using XmppBot.Plugins.Salesforce.Tasks.Sets;

namespace XmppBot.Plugins.Salesforce
{
    [Export(typeof(XmppBotPluginBase))]
    public class TNDPlugin : XmppBotPluginBase
    {
        public TNDPlugin() : base("tnd")
        {
            this.Tasks.Add(new GetQueryTask(this.Name));
            this.Tasks.Add(new GetApiTask(this.Name));
            this.Tasks.Add(new GetDomainTask(this.Name));
            this.Tasks.Add(new GetVersion(this.Name));
            this.Tasks.Add(new SetMachineIPsTask(this.Name));
            this.Tasks.Add(new FixMachineIPsTask(this.Name));
        }
    }
}
