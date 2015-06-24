using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Web;

namespace aMuleCtrl.Database
{
    class Designation
    {
        public static SQLiteConnection Initialize()
        {
            string dbPath = "Data Source =" + Config.GetInstance().GetDatabaseFile();
            SQLiteConnection conn = new SQLiteConnection(dbPath);
            conn.Open();//打开数据库，若文件不存在会自动创建 
            string sql = "CREATE TABLE IF NOT EXISTS designations(id integer NOT NULL PRIMARY KEY AUTOINCREMENT, " + 
                "designation varchar(64) UNIQUE, title varchar(1024), downloaded integer DEFAULT 0, artist_id integer DEFAULT 0);";
            SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn); 
            cmdCreateTable.ExecuteNonQuery();
            return conn;
        }

        public static List<String> GetAllDesignations()
        {
            using (SQLiteConnection conn = Initialize())
            {
                List<String> ret = new List<string>();
                String sql = "SELECT * FROM designations";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(HttpUtility.HtmlDecode(reader.GetString(1)));
                    }
                }
                return ret;
            }
        }

        public static Boolean AddDesignation(String designation, String title = null, Boolean downloaded = false, String artist = null)
        {
            if (title == null)
                title = designation;
            int artistId = 0;
            if (artist != null)
                artistId = Artist.GetArtistByName(artist);
               
   
            String sql = "INSERT INTO designations (designation, title, downloaded, artist_id) VALUES ('" + 
                HttpUtility.HtmlEncode(designation) + "', '" + HttpUtility.HtmlEncode(title) + "', " + (downloaded ? 1 : 0) + ", " + artistId + ")";

            using (SQLiteConnection conn = Initialize())
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException)
                {
                    if (!Config.GetInstance().IsSilent())
                    {
                        Console.WriteLine("Designation exists.");
                    }
                    return false;
                }
            }
            return true;
        }

    }
}
