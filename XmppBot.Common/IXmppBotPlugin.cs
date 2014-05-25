using System.Collections;
using System.Collections.Generic;

namespace XmppBot.Common
{
    public interface IXmppBotPlugin
    {
        string Name { get; }

        IEnumerable<string> TaskKeys { get; }

        string ExecuteTask(ParsedLine taskInfo);
    }
}
