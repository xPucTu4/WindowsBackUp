using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindowsBackUp
{
    [Serializable]
    [XmlRoot(ElementName="BackUpConfig")]
    public class AppConfigData
    {
        public AppConfigData()
        {
            this.Database.Databases = new List<string>();
        }

        [XmlAttribute]
        public Boolean ThisIsSample = true;

        public String TempDirectory
        {
            get
            {
                if (this.tempDirectory == null)
                {
                    this.tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                }
                Directory.CreateDirectory(tempDirectory);
                return tempDirectory;
            }
        }
        [XmlIgnore]
        private String tempDirectory = null;
        public String TargetDirectory = String.Empty;
        [XmlAttribute]
        public Boolean Compress = false;

        public List<Items.ItemExec> Execs = new List<Items.ItemExec>();
        public List<Items.ItemFile> Files = new List<Items.ItemFile>();
        public List<Items.ItemDirectory> Directories = new List<Items.ItemDirectory>();

        public Items.ItemDatabase Database = new Items.ItemDatabase();
    }
}
