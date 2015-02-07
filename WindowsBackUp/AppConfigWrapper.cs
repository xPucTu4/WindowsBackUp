using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WindowsBackUp
{
    public static class AppConfigWrapper
    {
        private static AppConfigData ConfigData = null;
        public static AppConfigData GetConfig()
        {
            return ConfigData;
        }

        /// <summary>
        /// The config file name
        /// </summary>
        public static readonly String ConfigFileName = "BackUpConfig.xml";

        static AppConfigWrapper()
        {
            string myDir = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName;
            FileInfo confFileInfo = new FileInfo(Path.Combine(myDir, ConfigFileName));
            if (!confFileInfo.Exists)
            {
                CreateConfig(confFileInfo);
            }

            ReadConfig(confFileInfo);
        }

        /// <summary>
        /// Create sample config file
        /// </summary>
        /// <param name="confFileInfo"></param>
        private static void CreateConfig(FileInfo confFileInfo)
        {
            Console.WriteLine("Creating sample config file...");

            AppConfigData acd = new AppConfigData();
            acd.TargetDirectory = @"\\192.168.0.10\Temp\kur4";

            // Exec commands
            acd.Execs.Add(new Items.ItemExec()
                {
                    CaptureOutput = true,
                    Command = "dir",
                    WorkingDirectory = "c:\\"
                });
            acd.Execs.Add(new Items.ItemExec()
            {
                CaptureOutput = false,
                Command = "ping 127.0.0.1",
                WorkingDirectory = null
            });

            acd.Execs.Add(new Items.ItemExec()
            {
                CaptureOutput = true,
                Command = "ping 8.8.8.8",
                WorkingDirectory = null,
                Stage = Items.ItemExec.RunStage.Before
            });

            acd.Execs.Add(new Items.ItemExec()
            {
                CaptureOutput = true,
                Command = "ping 8.8.8.8",
                WorkingDirectory = null,
                Stage=Items.ItemExec.RunStage.After
            });

            // Backup directories
            acd.Directories.Add(new Items.ItemDirectory(@"c:\Windows"));
            acd.Directories.Add(new Items.ItemDirectory()
                {
                    FullPath = @"c:\Program Files (x86)",
                    Compress = true
                });

            // Backup files
            acd.Files.Add(new Items.ItemFile(@"c:\autoexec.bat"));


            // MS SQL
            acd.Database.Username = "sa";
            //acd.Database.Password = "kur123";
            acd.Database.Databases.Add("hMailServer");
            acd.Database.Databases.Add("tfs123");


            XmlSerializer ser = new XmlSerializer(typeof(AppConfigData));

            // Xml Settings
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.NewLineOnAttributes = false;

            // Write to file
            using (FileStream confFileStream = confFileInfo.Create())
            {
                using (XmlWriter confWriter = XmlWriter.Create(confFileStream, settings))
                {
                    ser.Serialize(confWriter, acd);
                    confWriter.Flush();
                }
            }

            // Exit
            Console.WriteLine("Press any key to continue, or any other key to quit.");
            Console.ReadKey(true);
            Console.WriteLine("Not that key. Press another one or this one again!");
            Console.ReadKey(true);
            Environment.Exit(0);

        }
        private static void ReadConfig(FileInfo confFileInfo)
        {
            GC.Collect();
            using (FileStream fs = confFileInfo.OpenRead())
            {
                XmlReader confReader = XmlReader.Create(fs);
                XmlSerializer ser = new XmlSerializer(typeof(AppConfigData));
                ConfigData = (AppConfigData)ser.Deserialize(confReader);
            }
        }
    }
}
