using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsBackUp
{
    static class Tools
    {
        /// <summary>
        /// The backup file name in format: year-DayOfYear-H-m
        /// </summary>
        /// <returns></returns>
        public static String CreateTargetName()
        {
            String[] parts = new String[]
            {
                DateTime.Now.Year.ToString(),
                DateTime.Now.DayOfYear.ToString(),
                DateTime.Now.Hour.ToString(),
                DateTime.Now.Minute.ToString()
            };
            return String.Join("-", parts);
        }
        public static String CreateConnectionString(string db, string username = null, string password = null)
        {
            Boolean IntegratedSecurity = (String.IsNullOrWhiteSpace(username));
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Data Source={0};", "localhost");
            if (IntegratedSecurity)
            {
                sb.Append("Integrated Security=True;");
            }
            else
            {
                sb.AppendFormat("User ID={0};", username);
                sb.AppendFormat("Password={0};", (String.IsNullOrWhiteSpace(password) ? "" : password));
            }
            sb.AppendFormat("Initial Catalog={0};", db);
            sb.Append("MultipleActiveResultSets=True;");
            sb.Append("Network Library=dbmssocn;"); // TCP/IP library
            sb.Append("Asynchronous Processing=True;");
            sb.Append("Packet Size=8192;");
            sb.Append("Pooling=False;");

            String connString = sb.ToString();
            return connString;

            return "";
        }

        internal static void CreateDirectories(AppConfigData conf)
        {
            foreach (string path in new string[] { conf.TargetDirectory, conf.TempDirectory })
            {
                try
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                catch { }
            }
            foreach(String s in new String[] {"Files", "Directories", "Databases", "Execs"})
            {
                string fullPath = Path.Combine(conf.TempDirectory, s);
                try
                {
                    Directory.CreateDirectory(fullPath);
                }
                catch { }
            }
        }


        public static void CopyDirectory(string Src, string Dst)
        {
            String[] Files;

            if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
                Dst += Path.DirectorySeparatorChar;
            if (!Directory.Exists(Dst)) Directory.CreateDirectory(Dst);
            Files = Directory.GetFileSystemEntries(Src);
            foreach (string Element in Files)
            {
                // Sub directories
                if (Directory.Exists(Element))
                    CopyDirectory(Element, Dst + Path.GetFileName(Element));
                // Files in directory
                else
                {
                    try
                    {
                        File.Copy(Element, Dst + Path.GetFileName(Element), true);
                    }
                    catch (Exception any)
                    {
                        Console.WriteLine("Error: {0}", any.Message);
                    }
                }
            }
        }
    }
}
