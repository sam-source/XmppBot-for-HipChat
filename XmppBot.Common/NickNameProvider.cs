using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace XmppBot.Common
{
    public class NickNameProvider
    {
        private BasicFileWrapper storage;

        public Dictionary<string, string> Index { get; set; }

        public NickNameProvider(BasicFileWrapper storage)
        {
            this.storage = storage;
        }

        public string GetName(string fullUserName)
        {
            if (this.Index == null) {
                this.LoadData();
            }

            if (this.Index.ContainsKey(fullUserName)) {
                return this.Index[fullUserName];
            }

            return "";
        }

        public void SaveName(string fullUserName, string nickName)
        {
            if (Index == null) {
                this.LoadData();
            }

            if (Index.ContainsKey(fullUserName)) {
                this.Index[fullUserName] = nickName;
            }
            else {
                this.Index.Add(fullUserName, nickName);
            }

            this.SaveData();
        }

        private void LoadData()
        {
            var rawXml = this.storage.GetContents();

            if (string.IsNullOrWhiteSpace(rawXml)) {
                this.Index = new Dictionary<string, string>();
                return;
            }

            var data = XElement.Parse(rawXml);

            var nodes = data.Descendants("nickName");

            this.Index = new Dictionary<string, string>();
            
            foreach (var node in nodes) {
                var fullName = node.Attribute("fullName").Value;
                var nickName = node.Value;

                this.Index.Add(fullName, nickName);
            }
        }

        private void SaveData()
        {
            var xmlString = new StringBuilder();
            xmlString.Append("<rootData>");
            
            foreach (var kvp in this.Index) {
                xmlString.Append("<nickName fullName=\"");
                xmlString.Append(kvp.Key);
                xmlString.Append("\">");
                xmlString.Append(kvp.Value);
                xmlString.Append("</nickName>");
            }

            xmlString.Append("</rootData>");

            this.storage.SaveContents(xmlString.ToString());
        }
    }
}
