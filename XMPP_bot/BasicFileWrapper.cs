using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace XMPP_bot
{
    public class BasicFileWrapper
    {
        private string dataFolder;


        private string filePath;

        public BasicFileWrapper(string dataFolder, string fileName)
        {
            this.dataFolder = dataFolder;
            this.filePath = Path.Combine(dataFolder, fileName);
        }

        public virtual string GetContents()
        {
            if (!File.Exists(this.filePath)) {
                return "";
            }

            return File.ReadAllText(this.filePath);
        }

        public virtual void SaveContents(string contents)
        {
            if (!Directory.Exists(this.dataFolder)) {
                Directory.CreateDirectory(this.dataFolder);
            }

            if (File.Exists(this.filePath)) {
                File.Delete(this.filePath);
            }

            using (var writer = File.CreateText(this.filePath)) {
                writer.Write(contents);
            }
        }
    }
}
