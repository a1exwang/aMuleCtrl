using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;  
using System.Data.OleDb;
using System.Data.SQLite;

namespace Ed2kDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            Database db = new Database(Environment.CurrentDirectory + "/test.sqlite");
            db.AddDesignation("ipz", "alex");
            db.AddDesignation("iptd", "google");
            db.AddLink("123124bsdfon23", "ipz");
            db.AddLink("oindfosi", "ipz");
            db.AddLink("dfslfj", "ipz");

            List<String> results = db.GetLinksByDesignation("ipz");
            foreach (String result in results)
            {
                db.IncreaseLinkDownloadTimes(result);
                Console.WriteLine(result);
            }

            db.Close();
        }



    }
}
