using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

using XmppBot.Common;

namespace XmppBot.Plugins
{
    /// <summary>
    /// The example.
    /// </summary>
    // [Export(typeof(IXmppBotMultiRoomPlugin))]
    public class BambooRoomListener : IXmppBotMultiRoomPlugin
    {
        public BambooRoomListener()
        {
            this.Name = "Bamboo Room Listener";
        }

        public MultiRoomMessage Evaluate(ParsedLine line, string roomInfo)
        {
            if (roomInfo.Contains("35954_bamboo_-_web_team")) {
                var result = new MultiRoomMessage();
                result.RoomId = "35954_web_dev_team@conf.hipchat.com/e v e";
                result.Message = line.Raw;
                return result;
            }

            return null;
        }

        public string Name { get; private set; }
    }
}
