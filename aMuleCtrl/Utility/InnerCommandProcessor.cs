using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aMuleCtrl.Database;
using aMuleCtrl.Link;

namespace aMuleCtrl
{
    class InnerCommandProcessor
    {
        public static void HelpMsg()
        {
            Console.WriteLine("Welcome to amule control console!");
            Console.WriteLine("Valid commands:");
            Console.WriteLine(
                "hlp \n" + 
                "typ (local|global|kad|filehash|urlmagnet) \n" + 
                "dat (designation|artist|title) \n" + 
                "src (keyword) \n" +
                "get  = get results and put into database\n" + 
                "clr \n" + 
                "scn (path) \n" + 
                "sav (d - designations|a - artist|+ - all) = save link/designation/artist to list.txt \n" + 
                "qqq = quit");
        }

        public Boolean InvalidCommand()
        {
            if (!Config.GetInstance().IsSilent())
                Console.WriteLine("Invalid Command!");
            return true;
        }

        static void SaveDesignations(FileSaver saver, int count, String artist = null)
        {
            List<String> designations = null;

            if(artist == null)
                designations = Designation.GetAllDesignations();
            else
                designations = Artist.GetDesignations(artist);
            
            foreach (String designation in designations)
            {
                // 获得所有的结果
                List<String> links = Ed2k.GetLinksByDesignation(designation);
                List<Ed2kLink> edlinks = new List<Ed2kLink>();
                foreach (String link in links)
                {
                    Ed2kLink el = Ed2kLink.Parse(link);
                    if (el != null)
                        edlinks.Add(el);
                }

                // 按照文件大小排序
                edlinks.Sort((lhs, rhs) =>
                {
                    return -((lhs.FileSize > rhs.FileSize) ? 1 : ((rhs.FileSize > lhs.FileSize) ? -1 : 0));
                });

                for (int i = 0; i < count && i < links.Count; ++i)
                {
                    saver.AddLink(edlinks[0].Link);
                }
            }
        }

        public Boolean Process(String str)
        {
            if (str.Length < 3)
            {
                return InvalidCommand();
            }

            String op = str.Substring(0, 3);
            String data = "";
            if (str.Length >= 4)
                data = str.Substring(4, str.Length - 4);

            if (op.Equals("clr", StringComparison.CurrentCultureIgnoreCase))
                Console.Clear();
            if (op.Equals("hlp", StringComparison.CurrentCultureIgnoreCase))
                HelpMsg();
            else if (op.Equals("sav", StringComparison.CurrentCultureIgnoreCase))
            {
                using (FileSaver save = new FileSaver(data))
                {
                    if (data.Equals("d", StringComparison.CurrentCultureIgnoreCase) || data.Equals("designations", StringComparison.CurrentCultureIgnoreCase))
                    {
                        SaveDesignations(save, 3);
                    }
                    else if (data.Equals("a", StringComparison.CurrentCultureIgnoreCase) || data.Equals("artists", StringComparison.CurrentCultureIgnoreCase))
                    {
                        List<String> artists = Artist.GetArtists();
                        foreach (String ar in artists)
                        {
                            SaveDesignations(save, 3);
                        }
                    }
                    else if (data.Equals("+", StringComparison.CurrentCultureIgnoreCase) || data.Equals("all", StringComparison.CurrentCultureIgnoreCase))
                    {
                        List<String> links = Database.Magnet.GetAllLinks();
                        links.AddRange(Database.Ed2k.GetAllLinks());

                        save.AddLinks(links.ToArray());
                    }
                    else
                    {
                        return InvalidCommand();
                    }
                }
            }
            else if (op.Equals("scn", StringComparison.CurrentCultureIgnoreCase))
            {
                LocalScanner scanner = LocalScanner.GetInstance();
                List<String[]> results = scanner.Scan(data);
                List<String> artists = Artist.GetArtists();
                // result[0] is designation, result[1] is filename

                int count = 0;
                foreach (String[] result in results)
                {
                    String artist = null;
                    foreach (String ar in artists)
                        if (result[1].Contains(ar))
                            artist = ar;

                    if (Designation.AddDesignation(result[0], result[1], true, artist))
                        ++count;
                }
                Console.WriteLine(count.ToString() + " designations added!");
            }
            else if (op.Equals("dat", StringComparison.CurrentCultureIgnoreCase))
            {
                Config.GetInstance().SetSearchDataType(data);
            }
            else if (op.Equals("qqq", StringComparison.CurrentCultureIgnoreCase))
                Environment.Exit(0);
            else
                return false;

            Config.GetInstance().AddLastCommand(op, data);
            return true;
        }
    }

}
