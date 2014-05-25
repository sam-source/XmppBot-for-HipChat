using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace XmppBot.Common
{
    public interface IXmppBotSequencePlugin
    {
        string Name { get; }

        IEnumerable<string> TaskKeys { get; }

        IObservable<string> ExecuteTask(ParsedLine taskInfo);
    }
}