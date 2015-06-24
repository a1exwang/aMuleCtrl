using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using aMuleCtrl.Link;
using aMuleCtrl.Database;

namespace aMuleCtrl
{
    class OutterCommandProcessor
    {
        public OutterCommandProcessor()
        {
            AMuleInterfaces.Initialize((_op, _data) => OnReceiveMessage(_op, _data));
        }

        public void SearchMagnet(String keyword)
        {
            if ("designation".Equals(Config.GetInstance().GetSearchDataType(), StringComparison.CurrentCulture))
            {
                List<MagnetLink> links = MagnetInterface.Search(keyword, Config.GetInstance().GetMagnetMaxCount());
                Dictionary<String, List<MagnetLink>> designations = ClassifyMagnets(links);
                SaveMagnet(designations);
            }
            else if ("title".Equals(Config.GetInstance().GetSearchDataType(), StringComparison.CurrentCulture))
            {
                String lastData = Config.GetInstance().GetLastCommands()[1][1];
                
                List<MagnetLink> links = MagnetInterface.Search(keyword, Config.GetInstance().GetMagnetMaxCount());

                Dictionary<String, List<MagnetLink>> designations = ClassifyMagnets(links);
                
                SaveMagnet(designations);
            }
            else if ("artist".Equals(Config.GetInstance().GetSearchDataType(), StringComparison.CurrentCulture))
            {
                String designation = Config.GetInstance().GetCurrentDesignation();
                List<MagnetLink> links = MagnetInterface.Search(keyword, Config.GetInstance().GetMagnetMaxCount());
                if (links.Count > 0)
                {
                    Dictionary<String, List<MagnetLink>> designations = new Dictionary<string, List<MagnetLink>>();
                    designations.Add(designation, links);

                    String artist = keyword;
                    Artist.AddArtist(artist);
                    SaveMagnet(designations, artist);
                }
            }
        }
        public Boolean Process(String op, String data)
        {
            Config.GetInstance().AddLastCommand(op, data);
            AMuleInterfaces amule = AMuleInterfaces.GetInstance();

            if (op.Equals("src", StringComparison.CurrentCultureIgnoreCase))
            {
                if (Config.GetInstance().GetSearchType().Equals("urlmagnet", StringComparison.CurrentCulture))
                {
                    SearchMagnet(data);
                }
                else
                {
                    if ("title".Equals(Config.GetInstance().GetSearchDataType(), StringComparison.CurrentCulture))
                    {
                        Config config = Config.GetInstance();

                        Designation.AddDesignation(data);
                        config.SetCurrentDesignation(data);
                    }
                    amule.SendMsg(op, data);
                }
            }
            else if (op.Equals("typ", StringComparison.CurrentCultureIgnoreCase))
            {
                Config.GetInstance().SetSearchType(data);
                if (!"urlmagnet".Equals(data, StringComparison.CurrentCulture))
                    amule.SendMsg(op, data);
            }
            else if (op.Equals("get", StringComparison.CurrentCultureIgnoreCase))
            {
                amule.SendMsg(op, data);
            }

            return false;
        }

        public static Dictionary<String, List<Ed2kLink>> ClassifyDesignations(List<Ed2kLink> links)
        {
            Dictionary<String, List<Ed2kLink>> ret = new Dictionary<string, List<Ed2kLink>>();
            foreach (Ed2kLink link in links)
            {
                String des = LocalScanner.GetDesignation(link.FileName);
                if (des != null)
                {
                    if (!ret.ContainsKey(des))
                        ret[des] = new List<Ed2kLink>();
                    ret[des].Add(link);
                }
            }
            return ret;
        }
        public void SaveEd2k(Dictionary<String, List<Ed2kLink>> data, String artist = null)
        {
            int count = 0;
            foreach (String designation in data.Keys)
            {
                Designation.AddDesignation(designation, null, false, artist);
                foreach (Ed2kLink edlink in data[designation])
                    if (Ed2k.AddLink(edlink.Link, designation))
                        ++count;
            }
            if (!Config.GetInstance().IsSilent())
                Console.WriteLine(count.ToString() + " links added!");
        }

        public static Dictionary<String, List<MagnetLink>> ClassifyMagnets(List<MagnetLink> links)
        {
            var ret = new Dictionary<string, List<MagnetLink>>();
            foreach (MagnetLink link in links)
            {
                String des = LocalScanner.GetDesignation(link.GetFileName());
                if (des != null)
                {
                    if (!ret.ContainsKey(des))
                        ret[des] = new List<MagnetLink>();
                    ret[des].Add(link);
                }
            }
            return ret;
        }
        public void SaveMagnet(Dictionary<String, List<MagnetLink>> data, String artist = null)
        {
            int count = 0;
            foreach (String designation in data.Keys)
            {
                Designation.AddDesignation(designation, null, false, artist);
                foreach (MagnetLink maglink in data[designation])
                    if (Magnet.AddLink(maglink.GetLink(), designation))
                        ++count;
            }
            if (!Config.GetInstance().IsSilent())
                Console.WriteLine(count.ToString() + " links added!");
        }
   
        public void OnReceiveMessage(String op, String data)
        {
            Config config = Config.GetInstance();

            Console.WriteLine(data);
            if (config.WillSaveToDb() && op.Equals("get", StringComparison.CurrentCultureIgnoreCase))
            {
                String lastOp = config.GetLastCommands()[1][0];
                String lastData = config.GetLastCommands()[1][1];

                String dataType = config.GetSearchDataType();

                // 根据当前搜索的数据类型, 存入数据库
                if (dataType.Equals("designation", StringComparison.CurrentCulture))
                {
                    List<Ed2kLink> links = AMuleInterfaces.GetEd2kLinks(data);
                    Dictionary<String, List<Ed2kLink>> designations = ClassifyDesignations(links);
                    SaveEd2k(designations);
                }
                else if (dataType.Equals("artist", StringComparison.CurrentCulture))
                {
                    String artist = lastData;
                    List<Ed2kLink> links = AMuleInterfaces.GetEd2kLinks(data);

                    Dictionary<String, List<Ed2kLink>> designations = ClassifyDesignations(links);
                    Artist.AddArtist(artist);

                    SaveEd2k(designations, artist);
                }
                else if (dataType.Equals("title", StringComparison.CurrentCulture))
                {
                    String designation = config.GetCurrentDesignation();
                    List<Ed2kLink> links = AMuleInterfaces.GetEd2kLinks(data);

                    Dictionary<String, List<Ed2kLink>> designations = new Dictionary<string, List<Ed2kLink>>();
                    designations.Add(designation, links);

                    SaveEd2k(designations);
                }
            }
        }
    }
}
