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
                line.NickName = CoreTasks.GetNickName(user);

                if (string.Equals(line.Command, "help", StringComparison.InvariantCultureIgnoreCase)) {
                    if (line.Args != null && line.Args.Length == 1) {
                        if (line.Args[0] == "core") {
                            SendMessage(msg.From, "------------ Core Tasks ------------", msg.Type);
                            System.Threading.Thread.Sleep(100);
                            SendMessage(msg.From, CoreTasks.GetListOfTasks(), msg.Type);
                            return;
                        }

                        var plugin = Plugins.FirstOrDefault(
                            p => string.Equals(p.Name, line.Args[0], StringComparison.InvariantCultureIgnoreCase));

                        if (plugin != null) {
                            var tasks =string.Join("\n", plugin.TaskKeys.Select(k => plugin.Name + "-" + k).ToArray());
                            SendMessage(msg.From, string.Format("------------ Plugin ({0}) Tasks ------------", plugin.Name), msg.Type);
                            System.Threading.Thread.Sleep(100);
                            SendMessage(msg.From, tasks, msg.Type);
                            return;
                        }

                        var seqPlugin =
                            SequencePlugins.FirstOrDefault(
                                p => string.Equals(p.Name, line.Args[0], StringComparison.InvariantCultureIgnoreCase));

                        if (seqPlugin != null) {
                            var tasks = string.Join("\n", seqPlugin.TaskKeys.Select(k => seqPlugin.Name + "-" + k).ToArray());
                            SendMessage(msg.From, string.Format("------------ Plugin ({0}) Tasks ------------", seqPlugin.Name), msg.Type);
                            System.Threading.Thread.Sleep(100);
                            SendMessage(msg.From, tasks, msg.Type);
                            return;
                        }

                        SendMessage(msg.From, string.Format("Plugin ({0}) not found.", line.Args[0]), msg.Type);
                        return;
                    }
                    else {
                        SendMessage(msg.From, "------------ Core Tasks ------------", msg.Type);
                        System.Threading.Thread.Sleep(100);
                        SendMessage(msg.From, CoreTasks.GetListOfTasks(), msg.Type);
                        System.Threading.Thread.Sleep(100);
                        SendMessage(msg.From, "------------ Plugin Tasks ------------", msg.Type);
                        System.Threading.Thread.Sleep(100);
                        var tasks = string.Join("\n", PluginTaskKeys.Select(kvp => kvp.Key).ToArray());
                        SendMessage(msg.From, tasks, msg.Type);

                        System.Threading.Thread.Sleep(100);
                        tasks = string.Join("\n", SequencePluginTaskKeys.Select(kvp => kvp.Key).ToArray());
                        SendMessage(msg.From, tasks, msg.Type);
                        return;
                    }
                }

                if (CoreTasks.ContainsCommand(line)) {
                    SendMessage(msg.From, CoreTasks.ExecuteCommand(line), msg.Type);
                    return;
                }

                if (PluginTaskKeys.ContainsKey(line.Command)) {
                    Task.Factory.StartNew(
                        () => SendMessage(msg.From, PluginTaskKeys[line.Command].ExecuteTask(line), msg.Type));
                    return;
                }

                if (SequencePluginTaskKeys.ContainsKey(line.Command)) {
                    Task.Factory.StartNew(
                        () => SendSequence(msg.From, SequencePluginTaskKeys[line.Command].ExecuteTask(line), msg.Type));
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
        public static IEnumerable<IXmppBotPlugin> Plugins { get; set; }

        [ImportMany(AllowRecomposition = true)]
        public static IEnumerable<IXmppBotSequencePlugin> SequencePlugins { get; set; }

        public static Dictionary<string, IXmppBotPlugin> PluginTaskKeys { get; set; }

        public static Dictionary<string, IXmppBotSequencePlugin> SequencePluginTaskKeys { get; set; }

        private static string LoadPlugins()
        {
            var container = new CompositionContainer(_catalog);
            Plugins = container.GetExportedValues<IXmppBotPlugin>();
            SequencePlugins = container.GetExportedValues<IXmppBotSequencePlugin>();

            PluginTaskKeys = new Dictionary<string, IXmppBotPlugin>();

            foreach (var plugin in Plugins) {
                foreach (var taskKey in plugin.TaskKeys) {
                    var key = plugin.Name + "-" + taskKey;

                    if (PluginTaskKeys.ContainsKey(key)) {
                        return "Duplicate task found. Loading Stopped. " + key;
                    }

                    PluginTaskKeys.Add(plugin.Name + "-" + taskKey, plugin);
                }
            }

            SequencePluginTaskKeys = new Dictionary<string, IXmppBotSequencePlugin>();

            foreach (var plugin in SequencePlugins) {
                foreach (var taskKey in plugin.TaskKeys) {
                    var key = plugin.Name + "-" + taskKey;

                    if (SequencePluginTaskKeys.ContainsKey(key)) {
                        return "Duplicate task found. Loading Stopped. " + key;
                    }

                    SequencePluginTaskKeys.Add(key, plugin);
                }
            }

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