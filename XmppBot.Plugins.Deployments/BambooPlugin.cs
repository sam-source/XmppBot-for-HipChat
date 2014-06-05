using XmppBot.Common;
using XmppBot.Plugins.Deployments.Tasks.Build;
using XmppBot.Plugins.Deployments.Tasks.Deploy;
using XmppBot.Plugins.Deployments.Tasks.Gets;

namespace XmppBot.Plugins.Deployments
{
    [System.ComponentModel.Composition.Export(typeof(XmppBotSequencePluginBase))]
    public class BambooPlugin : XmppBotSequencePluginBase
    {
        public BambooPlugin() : base("bamboo")
        {
            //this.Tasks.Add(new BuildTask(this.Name));
            //this.Tasks.Add(new GetBuildStatusTask(this.Name));
            this.Tasks.Add(new DeployContentTask(this.Name));
        }
    }
}
