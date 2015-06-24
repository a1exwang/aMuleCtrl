using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Web;

namespace aMuleCtrl.Database
{
    class Ed2k
    {
        public static SQLiteConnection Initialize()
        {
            string dbPath = "Data Source =" + Config.GetInstance().GetDatabaseFile();
            SQLiteConnection conn = new SQLiteConnection(dbPath);
            conn.Open();//打开数据库，若文件不存在会自动创建  
            string sql = "CREATE TABLE IF NOT EXISTS ed2ks(id integer NOT NULL PRIMARY KEY AUTOINCREMENT, " + 
                "link VARCHAR(1024) UNIQUE, designation_id INTEGER, download_times integer DEFAULT 0);";
            SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn);
            cmdCreateTable.ExecuteNonQuery();
            return conn;
        }

        public static Boolean AddLink(String link, String designation)
        {
            using (SQLiteConnection conn = Initialize())
            {
                
                String sql = "SELECT * FROM designations WHERE designation = '" + HttpUtility.HtmlEncode(designation) + "'";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int designation_id = reader.GetInt32(0);
                        sql = "INSERT INTO ed2ks (link, designation_id) VALUES ('" + HttpUtility.HtmlEncode(link) + "', " + designation_id + ")";
                        cmd = new SQLiteCommand(sql, conn);
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (SQLiteException)
                        {
                            if (!Config.GetInstance().IsSilent())
                                Console.WriteLine("link exists!");
                            return false;
                        }
                    }
                    else
                    {
                        conn.Close();
                        throw new ArgumentException("designation not found!");
                    }
                }
            }
            return true;
        }
        public static void RemoveLink(String link)
        {
            using (SQLiteConnection conn = Initialize())
            {
                String sql = "DELETE * FROM ed2ks WHERE link = '" + HttpUtility.HtmlEncode(link) + "'";
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }
        public static List<String> GetAllLinks()
        {
            List<String> ret = new List<string>();
            SQLiteConnection conn = Initialize();
            String sql = "SELECT * FROM ed2ks";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            using (SQLiteDataReader linksReader = cmd.ExecuteReader())
            {
                while (linksReader.Read())
                {
                    ret.Add(HttpUtility.HtmlDecode(linksReader.GetString(1)));
                }
            }
            return ret;
        }

        public static List<String> GetLinksByDesignation(String designation)
        {
            List<String> ret = new List<string>();
            String sql = "SELECT * FROM designations WHERE designation = '" + HttpUtility.HtmlEncode(designation) + "'";
            using (SQLiteConnection conn = Initialize())
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int designation_id = reader.GetInt32(0);
                        sql = "SELECT * FROM ed2ks WHERE designation_id = " + designation_id;
                        cmd = new SQLiteCommand(sql, conn);
                        using (SQLiteDataReader linksReader = cmd.ExecuteReader())
                        {
                            while (linksReader.Read())
                            {
                                ret.Add(HttpUtility.HtmlDecode(linksReader.GetString(1)));
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException("designation not found!");
                    }
                }
                return ret;
            }
        }

        public static void IncreaseLinkDownloadTimes(String link)
        {
            List<String> ret = new List<string>();
            String sql = "SELECT * FROM ed2ks WHERE link = '" + HttpUtility.HtmlEncode(link) + "'";

            using (SQLiteConnection conn = Initialize())
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        int times = reader.GetInt32(3) + 1;
                        cmd = new SQLiteCommand("UPDATE ed2ks SET download_times = " + times + " WHERE id = " + id, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

    }
}
