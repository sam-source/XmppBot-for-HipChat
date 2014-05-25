using System;
using System.IO;

using XmppBot.Common;

namespace XMPP_bot.Tasks
{
    class GetStorageLocationTask : SimpleTaskBase
    {
        public override string Name
        {
            get
            {
                return "get-storagelocation";
            }
        }

        public override string Execute(ParsedLine taskInfo)
        {
            if (!this.Match(taskInfo.Command)) {
                return "Invalid command.";
            }

            return taskInfo.User + ", my data is stored here: "
                            + Path.Combine(Environment.CurrentDirectory, "Data");
        }
    }
}
