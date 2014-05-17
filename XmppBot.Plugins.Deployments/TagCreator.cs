using System;

using SharpSvn;

namespace XmppBot.Plugins.Deployments
{
    public class TagCreator
    {
        public string GetTagName()
        {
            return "trunk_";
        }

        public string GetTagUrl()
        {
            var url = "https://devrepo.tessituranetworkdev.com/svn/TNEW2/trunk/" + this.GetTagName();
            return url;
        }

        public bool TagExists(string url)
        {
            try {
                var svnclient = new SvnClient();
                SvnTarget source = new SvnUriTarget(url);
                return svnclient.Info(
                    source, new SvnInfoArgs { Depth = SvnDepth.Empty },
                    (o, e) =>
                    {
                        Console.WriteLine("Here");
                    });
            }
            catch (SharpSvn.SvnRepositoryIOException) {
                return false;
            }
        }

        public void CreateTag()
        {
            var tagsPath = "";
            
            var svnclient = new SvnClient();
            SvnTarget source = new SvnUriTarget("https://devrepo.tessituranetworkdev.com/svn/TNEW2/trunk");
            var target = new Uri("");
            // svnclient.RemoteCopy(source, )
        }
    }
}
