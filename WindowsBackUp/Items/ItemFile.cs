using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsBackUp.Items
{
    public struct ItemFile : IFSEntry
    {
        public ItemFile(string path) : this()
        {
            this.FullPath = path;
        }
        public string FullPath { get; set; }
    }
}
