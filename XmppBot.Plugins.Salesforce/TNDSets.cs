﻿namespace XmppBot.Plugins.Salesforce
{
    public class TNDSets
    {
        public string MachineIPs(string user, string[] args)
        {
            var client = new TND.Client();
            
            client.UpdateOpportunityMachineIPs(args[0], args[1], args[2]);

            var tnewClient = new TND.TNEWProvider();
            
            var op = tnewClient.GetRecord(args[0]);

            return string.Format("{0} the record was saved. {1}", user, string.Join(",", op.MachineIPAddresses));
        }
    }
}
