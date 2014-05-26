using System;
using System.IO;
using System.Text;

using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class GetStorageLocationTask : SimpleTaskBase
    {
        public GetStorageLocationTask()
            : base(null, "get-storagelocation") { }

        protected override string ExecuteTask(ParsedLine taskInfo)
        {
            return taskInfo.User + ", my data is stored here: "
                            + Path.Combine(Environment.CurrentDirectory, "Data");
        }

        protected override string HelpDescription
        {
            get
            {
                return "This command will respond with the location where the bot stores data. This command is for internal diagnostics.";
            }
        }

        protected override string HelpExample
        {
            get
            {
                return "!get-storagelocation";
            }
        }

        protected override string HelpFormat
        {
            get
            {
                return "The command has no parameters.";
            }
        }
    }
}
