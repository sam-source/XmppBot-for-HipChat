namespace XmppBot.Common
{
    public interface IXmppBotPlugin
    {
        string Evaluate(ParsedLine line);
        string Name { get; }
    }

    public interface IXmppBotMultiRoomPlugin
    {
        MultiRoomMessage Evaluate(ParsedLine line, string roomInfo);
        string Name { get; }
    }

    public class MultiRoomMessage
    {
        public string Message { get; set; }

        public string RoomId { get; set; }
    }
}
