using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;

namespace Ed2kDatabase
{
    class Database
    {
        public Database(String path = "test.db")
        {
            InitTables(path);
        }

        public void AddLink(String link, String designation)
        {
            String sql = "SELECT * FROM designations WHERE designation = '" + designation + "'";
            SQLiteCommand cmd = new SQLiteCommand(sql, mConn);
            using (SQLiteDataReader reader = cmd.ExecuteReader()) 
            {
                if(reader.Read())
                {
                    int designation_id = reader.GetInt32(0);
                    sql = "INSERT INTO links (link, designation_id) VALUES ('" + link + "', " + designation_id + ")";
                    cmd = new SQLiteCommand(sql, mConn);
                    cmd.ExecuteNonQuery();
                }
                else
                {
                    throw new ArgumentException("designation not found!");
                }
            }
        }
        public void RemoveLink(String link)
        {
            String sql = "DELETE * FROM links WHERE link = '" + link + "'";
            SQLiteCommand cmd = new SQLiteCommand(sql, mConn);
            cmd.ExecuteNonQuery();
        }
        public void IncreaseLinkDownloadTimes(String link)
        {
            List<String> ret = new List<string>();
            String sql = "SELECT * FROM links WHERE link = '" + link + "'";
            SQLiteCommand cmd = new SQLiteCommand(sql, mConn);
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int times = reader.GetInt32(3) + 1;
                    cmd = new SQLiteCommand("UPDATE links SET download_times = " + times + " WHERE id = " + id, mConn);
                    cmd.ExecuteNonQuery();
                }
            }

        }
        public void AddDesignation(String designation, String title)
        {
            String sql = "INSERT INTO designations (designation, title) VALUES ('" + designation + "', '" + title + "')";
            SQLiteCommand cmd = new SQLiteCommand(sql, mConn);
            cmd.ExecuteNonQuery();
        }
        public List<String> GetLinksByDesignation(String designation)
        {
            List<String> ret = new List<string>();
            String sql = "SELECT * FROM designations WHERE designation = '" + designation + "'";
            SQLiteCommand cmd = new SQLiteCommand(sql, mConn);
            using (SQLiteDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    int designation_id = reader.GetInt32(0);
                    sql = "SELECT * FROM links WHERE designation_id = " + designation_id;
                    cmd = new SQLiteCommand(sql, mConn);
                    using (SQLiteDataReader linksReader = cmd.ExecuteReader())
                    {
                        while (linksReader.Read())
                        {
                            ret.Add(linksReader.GetString(1));
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

        public void Close()
        {
            mConn.Close();
        }

        void InitTables(String path)
        {
            string dbPath = "Data Source =" + path;
            SQLiteConnection conn = new SQLiteConnection(dbPath);
            conn.Open();//打开数据库，若文件不存在会自动创建  

            string sql = "CREATE TABLE IF NOT EXISTS links(id integer NOT NULL PRIMARY KEY AUTOINCREMENT, link VARCHAR(1024) UNIQUE, designation_id INTEGER, download_times integer DEFAULT 0);";
            SQLiteCommand cmdCreateTable = new SQLiteCommand(sql, conn);
            cmdCreateTable.ExecuteNonQuery();

            sql = "CREATE TABLE IF NOT EXISTS designations(id integer NOT NULL PRIMARY KEY AUTOINCREMENT, designation varchar(64) UNIQUE, title varchar(1024));";
            cmdCreateTable = new SQLiteCommand(sql, conn);
            cmdCreateTable.ExecuteNonQuery();
            mConn = conn;
        }

        SQLiteConnection mConn;
    }
}
