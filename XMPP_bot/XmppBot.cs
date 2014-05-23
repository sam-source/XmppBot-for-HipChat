﻿using System;
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
using XmppBot.Common;
using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.roster;
using agsXMPP.protocol.x.muc;

using XMPP_bot.User;

namespace XMPP_bot
{
    class XmppBot
    {
        private static DirectoryCatalog _catalog = null;
        private static XmppClientConnection _client = null;
        private static Dictionary<string, string> _roster = new Dictionary<string, string>(20);

        private static NickNameProvider NickNameProvider;
        static XmppBot()
        {
            var fileWrapper = new BasicFileWrapper(Path.Combine(Environment.CurrentDirectory, "Data"), "NickNames.xml");
            NickNameProvider = new NickNameProvider(fileWrapper);
        }

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
            if (!String.IsNullOrEmpty(msg.Body))
            {
                Console.WriteLine("Message : {0} - from {1}", msg.Body, msg.From);

                string user;
                string originalUserName;
                if (msg.Type != MessageType.groupchat)
                {
                    if (msg.From == null || msg.From.User == null) {
                        user = "Unknown User";
                    }
                    else {
                        if (!_roster.TryGetValue(msg.From.User, out user)) {
                            user = "Unknown User";
                        }
                    }
                }
                else
                {
                    user = msg.From.Resource;
                }

                if (user == ConfigurationManager.AppSettings["RoomNick"])
                    return;

                originalUserName = user;
                var nickName = NickNameProvider.GetName(user);

                if (!string.IsNullOrWhiteSpace(nickName)) {
                    user = nickName;
                }

                ParsedLine line = new ParsedLine(msg.Body.Trim(), user, ConfigurationManager.AppSettings["BotHandle"]);

                switch (line.Command)
                {
                    case "where":
                        if (line.Args.Length == 4 && line.Args[0] == "is" && line.Args[1] == "your"
                            && line.Args[2] == "data" && line.Args[3] == "stored?") {
                                SendMessage(msg.From, user + ", my data is stored here: " + Path.Combine(Environment.CurrentDirectory, "Data"), msg.Type);
                        }
                        return;
                    case "call":
                        if (line.Args.Length > 1 && line.Args[0] == "me") {
                            string nick = "";
                            for (int i = 1; i < line.Args.Length; i++) {
                                if (i > 1) {
                                    nick += " ";
                                }
                                nick += line.Args[i];
                            }

                            NickNameProvider.SaveName(originalUserName, nick);
                            
                            SendMessage(msg.From, "Ok " + nick, msg.Type);
                        }

                        return;
                    case "leave":
                        SendMessage(msg.From, "If you insist... :'(", msg.Type);
                        System.Threading.Thread.Sleep(2000);
                        Environment.Exit(1);
                        return;

                    case "reload":
                        SendMessage(msg.From, LoadPlugins(), msg.Type);
                        break;

                    default:
                        Task.Factory.StartNew(() =>
                                              Parallel.ForEach(Plugins,
                                                  plugin => SendMessage(msg.From, plugin.Evaluate(line), msg.Type)
                                                  ));

                        Task.Factory.StartNew(() =>
                                              Parallel.ForEach(SequencePlugins,
                                                  plugin => SendSequence(msg.From, plugin.Evaluate(line), msg.Type)
                                                  ));

                        Task.Factory.StartNew(() =>
                                              Parallel.ForEach(MultiRoomPlugins,
                                                  plugin =>
                                                  {
                                                      var temp = plugin.Evaluate(line, msg.From.Bare);
                                                      if (temp == null) {
                                                          return;
                                                      }
                                                      SendMessage(new Jid(temp.RoomId), temp.Message, msg.Type);
                                                  }));
                        break;
                }
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

        [ImportMany(AllowRecomposition = true)]
        public static IEnumerable<IXmppBotMultiRoomPlugin> MultiRoomPlugins { get; set; }

        private static string LoadPlugins()
        {
            var container = new CompositionContainer(_catalog);
            Plugins = container.GetExportedValues<IXmppBotPlugin>();
            SequencePlugins = container.GetExportedValues<IXmppBotSequencePlugin>();
            MultiRoomPlugins = container.GetExportedValues<IXmppBotMultiRoomPlugin>();

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Loaded the following plugins");
            foreach (var part in _catalog.Parts)
            {
                builder.AppendFormat("\t{0}\n", part.ToString());
            }

            return builder.ToString();
        }
    }
}