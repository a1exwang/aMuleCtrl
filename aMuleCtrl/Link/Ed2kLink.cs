using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web;

namespace aMuleCtrl
{
    class Ed2kLink
    {
        // 解析ed2k链接, 拆分成文件名, 大小和hash
        public static Ed2kLink Parse(String link)
        {
            if (link == null) return null;
            Regex regex = new Regex(regexEd2k, RegexOptions.IgnoreCase);
            Match match = regex.Match(link);
            if (!match.Success)
                return null;

            return new Ed2kLink(match.Groups[1].ToString(), UInt64.Parse(match.Groups[2].ToString()), match.Groups[3].ToString());
        }
        private Ed2kLink(String fileName, UInt64 size, String hash)
        {
            FileName = HttpUtility.UrlDecode(fileName);
            FileSize = size;
            FileHash = hash;
        }
        public String Link
        {
            get
            {
                return "ed2k://|file|" + FileName + "|" + FileSize.ToString() + "|" + FileHash + "|/";
            }
        }
        public UInt64 FileSize
        {
            get;
            private set;
        }
        public String FileHash
        {
            get;
            private set;
        }

        public String FileName
        {
            get;
            private set;
        }

        private const String regexEd2k = @"ed2k://\|file\|(.*)\|([0-9]+)\|([A-Za-z0-9]+)\|/";

    }
}
