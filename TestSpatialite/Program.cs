using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace TestSpatialite
{
    class Program
    {
        public static string getSqliteV3ConnectionString(string sqlitePath)
        {
            //Default Timeout is in seconds, useful for when sqlite is locked,
            //exception is returned after 1 second, default is 30 seconds.            
            return "Data source=" + sqlitePath + "; Version = 3; Default Timeout = 3;";
        }

        static bool is64bitProcess()
        {
            return (IntPtr.Size == 8);
        }

        static bool is32bitProcess()
        {
            return (IntPtr.Size == 4);
        }

        static void Main(string[] args)
        {
            string mod_spatialite_folderPath = "mod_spatialite-4.3.0a-win-amd64";

            if (is64bitProcess())
            {
                Console.WriteLine("64bit process");
                mod_spatialite_folderPath = "mod_spatialite-4.3.0a-win-amd64";                
            }
            else if(is32bitProcess())
            {
                Console.WriteLine("32bit process");
                mod_spatialite_folderPath = "mod_spatialite-4.3.0a-win-x86";                
            }
                        
            //using relative path, cannot use absolute path, dll load will fail
            //string mod_spatialite_dllPath = @"mod_spatialite-4.3.0a-win-amd64\mod_spatialite";            
            string path = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine) + ";" + mod_spatialite_folderPath;
            Environment.SetEnvironmentVariable("Path", path, EnvironmentVariableTarget.Process);
                        
            string sqlitePath = "_BCC_replication.sqlite";

            //select 'geometry::STGeomFromWKB(0x' || hex(ST_AsBinary(geometry)) || ', 28356) ' as geometry2  from qpp_airport_ols;            
            string sqliteConnectionString = getSqliteV3ConnectionString(sqlitePath);
            
            using (var sqliteConnection = new SQLiteConnection(sqliteConnectionString))
            {
                sqliteConnection.Open();
                Console.WriteLine("Load-ing mod_spatialite");
                string mod_spatialite_dllPath = mod_spatialite_folderPath + @"\mod_spatialite";
                sqliteConnection.LoadExtension(mod_spatialite_dllPath);
                Console.WriteLine("Load-ed mod_spatialite");                
                sqliteConnection.Execute("update qpp_airport_ols set lga_code = 111122 where ogc_fid = 2;");                
            }
            
            Console.ReadLine();
        }
    }
}
