using System;

using XmppBot.Common;

namespace XmppBot.Plugins.Conversation
{
    [System.ComponentModel.Composition.Export(typeof(IXmppBotPlugin))]
    public class TuringPlugin : IXmppBotPlugin
    {
        public string Evaluate(ParsedLine line)
        {
            string user = line.User;

            if (line.Raw.Contains(line.BotHandle)) {
                if (!line.IsCommand) {
                    if (line.Raw.StartsWith("hi " + line.BotHandle, StringComparison.InvariantCultureIgnoreCase) ||
                        line.Raw.StartsWith("hello " + line.BotHandle, StringComparison.InvariantCultureIgnoreCase)) {
                        var replies = new[] { "hi {0}", "hello {0}", "hi {0}, how are you?" };
                        return string.Format(replies[(new Random((int)DateTime.Now.Ticks)).Next(0, 3)], user);
                    }

                    if (line.Raw.StartsWith("thanks " + line.BotHandle, StringComparison.InvariantCultureIgnoreCase)
                        || line.Raw.StartsWith("thank you " + line.BotHandle, StringComparison.InvariantCultureIgnoreCase)) {
                        return "you're welcome " + user;
                    }

                    if (line.Raw.StartsWith("let's get to work " + line.BotHandle, StringComparison.InvariantCultureIgnoreCase)) {
                        return "/me rolls up her sleeves!";
                    }
                }
            }

            return null;
        }

        public string Name
        {
            get
            {
                return "Conversation";
            }
        }
    }
}
