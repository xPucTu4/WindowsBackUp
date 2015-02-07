using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsBackUp.Items
{
    public struct ItemDirectory : IFSEntry
    {
        public ItemDirectory(string path) : this()
        {
            this.FullPath = path;
            this.Compress = false;
        }

        public ItemDirectory(string path, bool compress) : this(path)
        {
            this.Compress = compress;
        }

        public string FullPath { get; set; }
        public Boolean Compress { get; set; }
    }
}
