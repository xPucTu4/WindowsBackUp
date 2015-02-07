using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindowsBackUp.Items
{
    public struct ItemDatabase
    {
        public ItemDatabase(string username, string password)
        {
            this.Username = username;
            this.Password = password;
            this.Databases = new List<string>();
        }

        [XmlElement(IsNullable=true)]
        public string Username;
        [XmlElement(IsNullable = true)]
        public string Password;
        public List<String> Databases;
    }
}
