using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aMuleCtrl.Link
{
    class MagnetLink
    {
        public MagnetLink(String fileName, String link)
        {
            mFileName = fileName;
            mLink = link;
        }
        public String GetFileName()
        {
            return mFileName;
        }
        public String GetLink()
        {
            return mLink;
        }

        String mFileName;
        String mLink;

    }
}
