using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WindowsBackUp.Items
{
    [Serializable]
    public struct ItemExec
    {
        public enum RunStage
        {
            Disabled = 0,
            Before = 1,
            After = 2
        }

        public RunStage Stage;

        public string Command;
        [XmlElement(IsNullable=true)]
        public string WorkingDirectory;

        [XmlAttribute]
        public bool CaptureOutput;
    }
}
