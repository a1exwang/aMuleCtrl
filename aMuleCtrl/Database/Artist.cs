using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Web;

namespace aMuleCtrl.Database
{
    class Artist
    {
        public static SQLiteConnection Initialize()
        {
            string dbPath = "Data Source =" + Config.GetInstance().GetDatabaseFile();
            SQLiteConnection conn = new SQLiteConnection(dbPath);
            conn.Open();//打开数据库，若文件不存在会自动创建  
            string sql = "CREATE TABLE IF NOT EXISTS artists(id integer NOT NULL PRIMARY KEY AUTOINCREMENT, " + 
                "name VARCHAR(1024) UNIQUE, height integer DEFAULT 0, cup varchar(50) DEFAULT '0');";
            SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn);
            cmdCreateTable.ExecuteNonQuery();
            return conn;
        }
        public static List<String> GetDesignations(String name)
        {
            var conn = Initialize();
            var ret = new List<String>();

            int artist_id = GetArtistByName(name);
            String sql = "SELECT * FROM designations WHERE artist = " + artist_id;
            var cmd = new SQLiteCommand(sql, conn);
            using (SQLiteDataReader linksReader = cmd.ExecuteReader())
            {
                while (linksReader.Read())
                {
                    ret.Add(HttpUtility.HtmlDecode(linksReader.GetString(1)));
                }
            }
            return ret;
        }
        public static int GetArtistByName(String name)
        {
            List<String> ret = new List<string>();
            String sql = "SELECT * FROM artists WHERE name = '" + HttpUtility.HtmlEncode(name) + "'";
            using (SQLiteConnection conn = Initialize())
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetInt32(0);
                    }
                    else
                    {
                        throw new ArgumentException("artist not found!");
                    }
                }
            }
        }
        public static void AddArtist(String name)
        {
            using (SQLiteConnection conn = Initialize())
            {
                String sql = "INSERT INTO links (name) VALUES ('" + HttpUtility.HtmlEncode(name) + "')";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException)
                {
                    if (!Config.GetInstance().IsSilent())
                        Console.WriteLine("artist already exists.");
                }
                
            }
        }
        public static List<String> GetArtists()
        {
            List<String> ret = new List<string>();
            String sql = "SELECT * FROM artists";
            using (SQLiteConnection conn = Initialize())
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ret.Add(reader.GetString(2));
                    }
                }
            }
            return ret;
        }
    }
}
