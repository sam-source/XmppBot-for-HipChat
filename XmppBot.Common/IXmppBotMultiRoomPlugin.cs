namespace XmppBot.Common
{
    public interface IXmppBotMultiRoomPlugin
    {
        MultiRoomMessage Evaluate(ParsedLine line, string roomInfo);
        string Name { get; }
    }
}