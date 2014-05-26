using System;
using System.Linq;

using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class GiveTask : SimpleTaskBase
    {
        public GiveTask() : base(null, "give") { }
        
        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            if (taskInfo.Args.Contains("dinosaur")) {
                return new Random(DateTime.Now.Second).Next(0, 5) >= 3 ? "(clevergirl)" : "(philosoraptor)";
            }

            if (taskInfo.Args.Contains("money")) {
                return ":$";
            }

            return "I have nothing to give, you took it all.";
        }

        protected override string HelpDescription
        {
            get
            {
                return "This command will make the bot give a something.";
            }
        }

        protected override string HelpExample
        {
            get
            {
                return "!give money";
            }
        }

        protected override string HelpFormat
        {
            get
            {
                return "The command has one parameter. The parameter is a variety of objects.";
            }
        }
    }
}
