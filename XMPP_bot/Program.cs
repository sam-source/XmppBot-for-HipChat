using System;
using System.Linq;
using Topshelf;

namespace XMPP_bot
{
    public class Program
    {
        public static void Main()
        {
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

            HostFactory.Run(x =>                                 
            {
                x.Service<XmppBot>(s =>
                    {
                        s.ConstructUsing(name => new XmppBot());
                        s.WhenStarted(xmppbot => xmppbot.Start());   
                        s.WhenStopped(xmppbot => xmppbot.Stop());     
                    });

                x.RunAsLocalSystem();                            

                x.SetDescription("XmppBot");        
                x.SetDisplayName("XmppBot");                       
                x.SetServiceName("XmppBot");                       
            });

            Console.ReadLine();
        }
    }
}
