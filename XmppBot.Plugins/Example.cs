using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

using XmppBot.Common;

namespace XmppBot.Plugins
{
    [Export(typeof(IXmppBotPlugin))]
    public class Example : IXmppBotPlugin
    {
        public string Evaluate(ParsedLine line)
        {
            if (!line.IsCommand) return string.Empty;

            switch (line.Command.ToLower())
            {
                case "thank":
                case "thanks":
                    return "You're welcome";
                case "give":
                    if (line.Args.Contains("dinosaur")) {
                        return new Random(DateTime.Now.Second).Next(0, 5) >= 3 ? "(clevergirl)" : "(philosoraptor)";
                    }

                    if (line.Args.Contains("money")) {
                        return ":$";
                    }

                    return "I have nothing to give, you took it all.";
                case "(highfive)":
                    return "(highfive)";
                case "@nathancampbell":
                    return "/me bows to @NathanCampbell's mighty wisdom!";

                case "smack":
                    if (line.Args.FirstOrDefault() == "@NathanCampbell") {
                        return "No way!";
                    }

                    if (line.Args.FirstOrDefault() == "@smenard") {
                        return "@eve does not hurt master.";
                    }

                    return String.Format("/me smacks {1} around with a large trout ... https://www.youtube.com/watch?v=IhJQp-q1Y1s", line.BotHandle, line.Args.FirstOrDefault() ?? "your mom");

                case "hug":
                    return String.Format("/me hugs {1} (awthanks)", line.BotHandle, line.Args.FirstOrDefault() ?? "themself");

                case "help":
                    return String.Format("Right now the only commands I know are !smack [thing] and !hug [thing].");

                default:
                    return null;
            }
        }

        public string Name
        {
            get { return "User Actions"; }
        }
    }
}
