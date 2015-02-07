using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace WindowsBackUp
{
    class Program
    {
        static void Main(string[] args)
        {
            AppConfigData conf = AppConfigWrapper.GetConfig();
            if (conf.ThisIsSample)
            {
                Console.WriteLine("This is the example configuration. Nothing will be backed up now.");
                Console.ReadKey(true);
                Environment.Exit(0);
            }

            Tools.CreateDirectories(conf);

            // Copy directories
            conf.Directories.ForEach(d =>
            {
                DirectoryInfo sourceDirInfo = new DirectoryInfo(d.FullPath);
                if (d.Compress)
                {
                    ZipFile.CreateFromDirectory(d.FullPath, Path.Combine(conf.TempDirectory, "Directories", sourceDirInfo.Name + ".zip"));
                }
                else
                {
                    Tools.CopyDirectory(d.FullPath, Path.Combine(conf.TempDirectory, "Directories", sourceDirInfo.Name));
                }
            });


            // Backup databases
            if (conf.Database.Databases.Any())
            {
                // Allow anyone to write here or mssql will not be able to dump the database
                DirectoryInfo dbDir = new DirectoryInfo(Path.Combine(conf.TempDirectory, "Databases"));
                FileSystemAccessRule fsar = new FileSystemAccessRule("Users", FileSystemRights.FullControl, AccessControlType.Allow);
                DirectorySecurity dsec = dbDir.GetAccessControl();
                dsec.AddAccessRule(fsar);
                dbDir.SetAccessControl(dsec);

                conf.Database.Databases.ForEach(db =>
                {
                    StringBuilder sqlCmdText = new StringBuilder();
                    sqlCmdText.Append("BACKUP DATABASE [");
                    sqlCmdText.Append(db);
                    sqlCmdText.Append("] TO  DISK = N'");
                    sqlCmdText.Append(Path.Combine(conf.TempDirectory, "Databases", db + ".bak"));
                    sqlCmdText.Append("' WITH NOFORMAT, NOINIT,  NAME = N'");
                    sqlCmdText.AppendFormat("{0} - Backup'", db);
                    sqlCmdText.Append(", SKIP, NOREWIND, NOUNLOAD,  STATS = 10");

                    using (SqlConnection sql = new SqlConnection(Tools.CreateConnectionString(db, conf.Database.Username, conf.Database.Password)))
                    {
                        sql.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = sqlCmdText.ToString();
                        cmd.CommandTimeout = 1800;
                        cmd.Connection = sql;
                        cmd.ExecuteNonQuery();
                        sql.Close();
                    }
                });
            }




            // Copy to destination
            if (conf.Compress)
            {
                ZipFile.CreateFromDirectory(conf.TempDirectory, Path.Combine(conf.TargetDirectory, Tools.CreateTargetName() + ".zip"));
            }
            else
            {
                Tools.CopyDirectory(conf.TempDirectory, Path.Combine(conf.TargetDirectory, Tools.CreateTargetName()));
            }

            // Delete temp files
            GC.Collect();
            System.Threading.Thread.Sleep(500);
            try
            {
                Directory.Delete(conf.TempDirectory, true);
            }
            catch(Exception any)
            {
                Console.WriteLine("Error: {0}", any.Message);
            }
        }
    }
}
