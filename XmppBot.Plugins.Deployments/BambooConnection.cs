using System;
using System.Collections.Generic;
using System.Net;
using System.Xml.Linq;

namespace XmppBot.Plugins.Deployments
{
    public class BambooConnection
    {
        private class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                
                request.Timeout = 5000;

                return request;
            }
        }
        private static Dictionary<string, string> PlanMapping;

        static BambooConnection()
        {
            PlanMapping = new Dictionary<string, string>();
            PlanMapping.Add("tnew+qa", "TNEXWEB-TQOD");
            PlanMapping.Add("tnew+live", "TNEXWEB-TNEWV45LIVELIGHT");
            //PlanMapping.Add("tnew+qa+aws", "APRAMP-AWSV45QALITE");
            //PlanMapping.Add("tnew+live+aws", "APRAMP-AT4LLV");
            //PlanMapping.Add("tnst+qa", "TNEXWEB-TQOD");
            //PlanMapping.Add("tnst+live", "TNEXWEB-TNEWV45LIVELIGHT");
        }

        private WebClient GetWebClient()
        {
            // curl -X POST --user eve:Tr1fl3! "http://bamboo.tessituranetwork.com:8085/rest/api/latest/queue/TNEXWEB-TNEWV45QALIGHT?os_authType=basic"

            var client = new MyWebClient();
            
            client.Headers.Add("Authorization", "Basic ZXZlOlRyMWZsMyE=");
            return client;
        }

        public string StartBuild(params string[] args)
        {
            var client = this.GetWebClient();
            
            var urlFormat = "http://bamboo.tessituranetwork.com:8085/rest/api/latest/queue/{0}?os_authType=basic&bamboo.variable.externalOrgCode={1}";

            try {
                var projectKey = this.GetPlanKey(args);

                var orgCode = args.Length > 3 ? args[3] : args[2];

                var url = string.Format(urlFormat, projectKey, orgCode);

                var result = client.UploadString(url, "");

                var parser = new BambooResultParser();
                
                return parser.ParseQueueResult(result);
            }
            catch (Exception ex) {
                return string.Format("There was an error while starting the build: {0}", ex.Message);
            }
        }

        public string GetPlanKey(string[] args)
        {
            var projectKey = string.Format("{0}+{1}", args[0].ToLower(), args[1].ToLower());

            if (args.Length > 3) {
                int temp;
                if (!int.TryParse(args[2], out temp)) {
                    projectKey += "+" + args[2].ToLower();
                }
            }

            if (PlanMapping.ContainsKey(projectKey)) {
                return PlanMapping[projectKey];
            }

            return null;
        }

        public string GetBuildState(string planKey, string buildNumber)
        {
            try {
                // curl --user eve:Tr1fl3! "http://bamboo.tessituranetwork.com:8085/rest/api/latest/result/TNEXWEB-TNEWV45QALIGHT/2722?os_authType=basic"
                var urlFormat =
                    "http://bamboo.tessituranetwork.com:8085/rest/api/latest/result/{0}/{1}?os_authType=basic";
                var url = string.Format(urlFormat, planKey, buildNumber);
                var client = this.GetWebClient();
                var xml = client.DownloadString(url);
                var parser = new BambooResultParser();

                return parser.ParseBuildStatusResult(xml, buildNumber, planKey);
            }
            catch (Exception ex) {
                return string.Format("Error while returning state: {0} - (After two errors, I will stop checking the status.)", ex.Message);
            }
        }

        public string DeployContent(string planKey, string orgCode, string buildType)
        {

            try {
                var urlFormat = "http://bamboo.tessituranetwork.com:8085/rest/api/latest/queue/{0}?os_authType=basic&bamboo.variable.orgCodes={1}&bamboo.variable.configType={2}";

                var url = string.Format(urlFormat, planKey, orgCode.ToUpper(), buildType.ToUpper());
                var client = this.GetWebClient();
                
                var xml = client.UploadString(url, "");

                var parser = new BambooResultParser();

                return parser.ParseQueueResult(xml);
            }
            catch (Exception ex) {
                return string.Format("Error while starting content deploy: {0} - (After two errors, I will stop checking the status.)", ex.Message);
            }
        }
    }
}
