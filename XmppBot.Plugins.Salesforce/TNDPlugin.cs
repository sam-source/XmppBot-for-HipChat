﻿using System.Collections.Generic;
using System.ComponentModel.Composition;

using XmppBot.Common;
using XmppBot.Plugins.Salesforce.Tasks;
using XmppBot.Plugins.Salesforce.Tasks.Fixes;
using XmppBot.Plugins.Salesforce.Tasks.Gets;
using XmppBot.Plugins.Salesforce.Tasks.Ping;
using XmppBot.Plugins.Salesforce.Tasks.Sets;

namespace XmppBot.Plugins.Salesforce
{
    [Export(typeof(XmppBotSequencePluginBase))]
    public class TNDPlugin : XmppBotSequencePluginBase
    {
        public TNDPlugin() : base("tnd")
        {
            this.Tasks.Add(new GetQueryTask(this.Name));
            this.Tasks.Add(new GetApiTask(this.Name));
            this.Tasks.Add(new GetDomainTask(this.Name));
            this.Tasks.Add(new GetVersion(this.Name));
            this.Tasks.Add(new GetMachineIPs(this.Name));
            this.Tasks.Add(new SetMachineIPsTask(this.Name));
            this.Tasks.Add(new FixMachineIPsTask(this.Name));
            this.Tasks.Add(new PingVersion(this.Name));
            this.Tasks.Add(new SetVersion(this.Name));
        }
    }
}
