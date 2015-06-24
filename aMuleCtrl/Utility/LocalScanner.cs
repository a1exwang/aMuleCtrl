using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace aMuleCtrl
{
    class LocalScanner
    {
        public static LocalScanner GetInstance() 
        {
            if (TheInstance == null)
                TheInstance = new LocalScanner();
            return TheInstance;
        }
        private static LocalScanner TheInstance = null;
        private LocalScanner() { }

        public List<String[]> Scan(String path)
        {
            List<String[]> ret = new List<string[]>();
            if (Directory.Exists(path))
            {
                String[] subDirs = Directory.GetDirectories(path);
                foreach (String dir in subDirs)
                    ret.AddRange(Scan(dir));

                String[] files = Directory.GetFiles(path);
                // 先把路径转换成文件名
                
                foreach (String dir in files)
                {
                    String fileName = GetFileNameByPath(dir);
                    String result = GetDesignation(fileName);
                    if(result != null)
                        ret.Add(new String[] { result, fileName } );
                }
            }
            return ret;
        }

        public static String[] GetDesignationParts(String fileName)
        {
            MatchCollection matches = regexDesignation.Matches(fileName);
            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    if (!CheckBlackList(m.Groups[1].ToString(), m.Groups[2].ToString()))
                    {
                        return new String[] { m.Groups[1].ToString(), m.Groups[2].ToString() };
                    }
                }
            }
            return null;
        }
        public static String GetDesignation(String fileName)
        {
            String[] ret = GetDesignationParts(fileName);
            if (ret == null) return null;
            return ret[0] + '-' + ret[1];
        }

        public static String GetFileNameByPath(String path)
        {
            Match matchFileName = regexFileName.Match(path);
            if (matchFileName.Success)
            {
                return matchFileName.Groups[1].ToString();
            }
            return "";
        }

        private static Boolean AreAllChar(String s)
        {
            foreach (char c in s)
            {
                if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
                    return false;
            }
            return true;
        }
        private static Boolean CheckBlackList(String p1, String p2)
        {
            Regex regex1 = new Regex("[0-9]*");
            Regex regex2 = new Regex("[A-Za-z]*");
            if (regex1.Match(p1).Success && p1.Length < 3 && regex1.Match(p2).Success && p2.Length < 4)
                return true;
            if (regex1.Match(p1).Success && p1.Length < 4 && regex1.Match(p2).Success && p2.Length < 3)
                return true;
            if (AreAllChar(p1) && AreAllChar(p2))
                return true;
            if (p1.IndexOf("pocket", StringComparison.CurrentCultureIgnoreCase) >= 0 || 
                p2.IndexOf("pocket", StringComparison.CurrentCultureIgnoreCase) >= 0)
                return true;
            return false;
        }

        private static Regex regexShortNumber = new Regex("[0-9]{0,2}[- +=~_]{0,5}[A-Za-z0-9]{0,2}");
        private static Regex regexFileName = new Regex(@"\\([^\\]+)$");
        private static Regex regexDesignation = new Regex("([A-Za-z0-9]{1,10})[- +=~_]{0,5}([A-Za-z0-9]{1,10})");
    }
}
