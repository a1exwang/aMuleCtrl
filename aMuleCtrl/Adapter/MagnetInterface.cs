using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using aMuleCtrl.Link;

namespace aMuleCtrl
{
    class MagnetInterface
    {
        public static List<MagnetLink> Search(String keyword, int max)
        {
            String result = Command.Execute("ruby", 
                    @"D:\App\Ruby\resmgr\obtain\p2ps.rb " + 
                    "urlmagnet " + 
                    "\"" + keyword + "\" " + 
                    max,
                    @"D:\App\Ruby\resmgr\obtain");
            StringReader reader = new StringReader(result);
            List<MagnetLink> ret = new List<MagnetLink>();

            while (true)
            {
                var name = reader.ReadLine();
                var link = reader.ReadLine();
                if (link == null || link == null) break;

                ret.Add(new MagnetLink(name, link));                
            }
            return ret;
        }
    }
}
