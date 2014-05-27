using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

using XmppBot;
using XmppBot.Common;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.roster;
using agsXMPP.protocol.x.muc;

namespace XMPP_bot
{
    class XmppBot
    {
        private static DirectoryCatalog _catalog = null;
        private static XmppClientConnection _client = null;
        private static Dictionary<string, string> _roster = new Dictionary<string, string>(20);

        public void Stop()
        {
        }

        public void Start()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (o, args) =>
            {
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                return loadedAssemblies.FirstOrDefault(asm => asm.FullName == args.Name);
            };

            string pluginsDirectory = Environment.CurrentDirectory + "\\plugins\\";
            if (!Directory.Exists(pluginsDirectory))
            {
                Directory.CreateDirectory(pluginsDirectory);
            }

            _catalog = new DirectoryCatalog(Environment.CurrentDirectory + "\\plugins\\");
            _catalog.Changed += new EventHandler<ComposablePartCatalogChangeEventArgs>(_catalog_Changed);
            var pluginList = LoadPlugins();

            Console.WriteLine(pluginList);

            _client = new XmppClientConnection(ConfigurationManager.AppSettings["Server"]);

            //_client.ConnectServer = "talk.google.com"; //necessary if connecting to Google Talk
            _client.AutoResolveConnectServer = false;

            _client.OnLogin += new ObjectHandler(xmpp_OnLogin);
            _client.OnMessage += new MessageHandler(xmpp_OnMessage);
            _client.OnError += new ErrorHandler(_client_OnError);

            Console.WriteLine("Connecting...");
            _client.Resource = ConfigurationManager.AppSettings["Resource"];
            _client.Open(ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]);
            Console.WriteLine("Connected.");

            _client.OnRosterStart += new ObjectHandler(_client_OnRosterStart);
            _client.OnRosterItem += new XmppClientConnection.RosterHandler(_client_OnRosterItem);
        }

        static void _client_OnError(object sender, Exception ex)
        {
            Console.WriteLine("Exception: " + ex);
        }

        static void _catalog_Changed(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            _catalog.Refresh();
        }

        static void _client_OnRosterStart(object sender)
        {
            _roster = new Dictionary<string, string>(20);
        }

        static void _client_OnRosterItem(object sender, RosterItem item)
        {
            if (!_roster.ContainsKey(item.Jid.User))
                _roster.Add(item.Jid.User, item.Name);
        }

        static void xmpp_OnLogin(object sender)
        {
            MucManager mucManager = new MucManager(_client);

            string[] rooms = ConfigurationManager.AppSettings["Rooms"].Split(',');

            foreach (string room in rooms)
            {
                Jid jid = new Jid(room + "@" + ConfigurationManager.AppSettings["ConferenceServer"]);
                mucManager.JoinRoom(jid, ConfigurationManager.AppSettings["RoomNick"]);
            }
        }

        private static void ProcessHelp(ParsedLine line, Message msg)
        {
            string tasks;
            int sleepTime = 150;

            if (line.Args.Length == 0) {
                SendMessage(msg.From, "------------ Core Tasks ------------", msg.Type);
                System.Threading.Thread.Sleep(sleepTime);
                SendMessage(msg.From, CoreTasks.GetListOfTasks(), msg.Type);
                System.Threading.Thread.Sleep(sleepTime);
                SendMessage(msg.From, "------------ Available Plugins ------------", msg.Type);
                System.Threading.Thread.Sleep(sleepTime);
                tasks = string.Join("\n", Plugins.Select(p => p.Name).ToArray());
                SendMessage(msg.From, tasks, msg.Type);

                System.Threading.Thread.Sleep(sleepTime);
                tasks = string.Join("\n", SequencePlugins.Select(p => p.Name).ToArray());
                SendMessage(msg.From, tasks, msg.Type);
                return;
            }

            var argument = line.Args[0];

            bool pluginFound =
                Plugins.Any(p => string.Equals(argument, p.Name, StringComparison.InvariantCultureIgnoreCase)) ||
                SequencePlugins.Any(p => string.Equals(argument, p.Name, StringComparison.InvariantCultureIgnoreCase));
        
            if (!pluginFound && string.Equals(argument, "core", StringComparison.InvariantCultureIgnoreCase)) {
                SendMessage(msg.From, "------------ Core Commands ------------", msg.Type);
                System.Threading.Thread.Sleep(sleepTime);
                SendMessage(msg.From, CoreTasks.GetListOfTasks(), msg.Type);
                return;
            }

            string header = "";

            if (!pluginFound && CoreTasks.GetTask(argument) != null) {
                header = string.Format("------------ Core ({0}) Command ------------", argument);
                SendMessage(msg.From, header, msg.Type);
                System.Threading.Thread.Sleep(sleepTime);
                SendMessage(msg.From, CoreTasks.GetTask(argument).GetHelp(), msg.Type);
                return;
            }

            XmppBotPluginBase plugin;
            XmppBotSequencePluginBase sequencePlugin;
            BotTaskBase pluginTask;
            
            tasks = "";
            string pluginName;
            bool isTask = argument.Contains("-");

            pluginName = isTask ? argument.Split('-')[0] : argument;

            plugin = Plugins.FirstOrDefault(p => string.Equals(p.Name, pluginName, StringComparison.InvariantCultureIgnoreCase));
            sequencePlugin = SequencePlugins.FirstOrDefault(p => string.Equals(p.Name, pluginName, StringComparison.InvariantCultureIgnoreCase));

            if (plugin != null) {
                if (isTask) {
                    pluginTask = plugin.GetTask(argument);
                    tasks = pluginTask.GetHelp();
                    header = string.Format("------------ Command ({0}) Help ------------", pluginTask.FullName);
                }
                else {
                    tasks = string.Join("\n", plugin.TaskKeys);
                    header = string.Format("------------ Plugin ({0}) Commands ------------", plugin.Name);
                }
            }

            if (sequencePlugin != null) {
                if (isTask) {
                    pluginTask = sequencePlugin.GetTask(argument);
                    tasks = pluginTask.GetHelp();
                    header = string.Format("------------ Command ({0}) Help ------------", pluginTask.FullName);
                } else {
                    tasks = string.Join("\n", sequencePlugin.TaskKeys);
                    header = string.Format("------------ Plugin ({0}) Commands ------------", sequencePlugin.Name);
                }
            }

            if (plugin == null && sequencePlugin == null) {
                SendMessage(msg.From, "Help information not found.", msg.Type);
                return;
            }

            SendMessage(msg.From, header, msg.Type);
            System.Threading.Thread.Sleep(sleepTime);
            SendMessage(msg.From, tasks, msg.Type);
        }

        static void xmpp_OnMessage(object sender, Message msg)
        {
            if (!String.IsNullOrEmpty(msg.Body)) {
                Console.WriteLine("Message : {0} - from {1}", msg.Body, msg.From);

                string user;

                if (msg.Type != MessageType.groupchat) {
                    if (msg.From == null || msg.From.User == null) {
                        user = "Unknown User";
                    }
                    else {
                        if (!_roster.TryGetValue(msg.From.User, out user)) {
                            user = "Unknown User";
                        }
                    }
                }
                else {
                    user = msg.From.Resource;
                }

                if (user == ConfigurationManager.AppSettings["RoomNick"]) {
                    return;
                }

                var line = new ParsedLine(msg.Body.Trim(), user, ConfigurationManager.AppSettings["BotHandle"]);

                if (!line.IsCommand) {
                    return;
                }

                line.NickName = CoreTasks.GetNickName(user);

                if (line.Command == "help") {
                    ProcessHelp(line, msg);
                    return;
                }

                if (CoreTasks.ContainsCommand(line)) {
                    SendMessage(msg.From, CoreTasks.ExecuteCommand(line), msg.Type);
                    return;
                }

                var pluginToExecute = Plugins.FirstOrDefault(p => p.Name == line.PreCommand);

                if (pluginToExecute != null) {
                    Task.Factory.StartNew(
                        () => SendMessage(msg.From, pluginToExecute.ExecuteTask(line), msg.Type));
                    return;
                }

                var sequencePluginToExecute = SequencePlugins.FirstOrDefault(p => p.Name == line.PreCommand);

                if (sequencePluginToExecute != null) {
                    Task.Factory.StartNew(
                        () => SendSequence(msg.From, sequencePluginToExecute.ExecuteTask(line), msg.Type));
                    return;
                }

                SendMessage(msg.From, "Unknown command.", msg.Type);
            }
        }

        public static void SendMessage(Jid to, string text, MessageType type)
        {
            if (text == null) return;

            _client.Send(new Message(to, type, text));
        }

        public static void SendSequence(Jid to, IObservable<string> messages, MessageType type)
        {
            if(messages == null)
            {
                return;
            }

            var observer = Observer.Create<string>(
                msg => SendMessage(to, msg, type),
                exception => Trace.TraceError(exception.ToString()));

            messages.Subscribe(observer);
        }

        [ImportMany(AllowRecomposition = true)]
        public static IEnumerable<XmppBotPluginBase> Plugins { get; set; }

        [ImportMany(AllowRecomposition = true)]
        public static IEnumerable<XmppBotSequencePluginBase> SequencePlugins { get; set; }

        private static string LoadPlugins()
        {
            var container = new CompositionContainer(_catalog);
            Plugins = container.GetExportedValues<XmppBotPluginBase>();
            SequencePlugins = container.GetExportedValues<XmppBotSequencePluginBase>();

            var builder = new StringBuilder();
            
            builder.AppendLine("Loaded the following plugins");

            foreach (var part in _catalog.Parts)
            {
                builder.AppendFormat("\t{0}\n", part);
            }

            return builder.ToString();
        }
    }
}