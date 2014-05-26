using System.Linq;

namespace XmppBot.Common
{
    public class ParsedLine
    {
        public ParsedLine(string line, string user, string botHandle)
        {
            ParseLine(line, botHandle);
            this.User = user;
            this.BotHandle = botHandle;
        }

        private void ParseLine(string line, string botHandle)
        {
            this.Raw = line;
            this.IsCommand = line.StartsWith("!") || line.StartsWith(botHandle);
            line = line.TrimStart('!');
            line = line.TrimStart(botHandle.ToCharArray());
            line = line.TrimStart();

            string[] parts = line.Split(' ');
            if (parts.Length <= 0)
            {
                this.Command = "invalid";
                this.Args = new string[] { };
                this.IsCommand = false;
                return;
            }

            this.Command = parts[0];
            this.Args = parts.Skip(1).ToArray();

            parts = this.Command.Split('-');

            if (parts.Length == 0) {
                return;
            }

            this.PreCommand = parts[0];
        }

        public string Command { get; private set; }

        public string PreCommand { get; private set; }

        public string Raw { get; private set; }

        public string[] Args { get; private set; }

        public bool IsCommand { get; private set; }

        public string User { get; private set; }

        public string BotHandle { get; private set; }

        private string innerNickName;

        public string NickName
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.innerNickName) ? this.User : this.innerNickName;
            }

            set
            {
                this.innerNickName = value;
            }
        }
    }
}
