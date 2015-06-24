using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.IO;

namespace WordStatistics
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 2)
            {
                String content = args[1];
                String conditions = args[0];

                String text = GetFileString(@"D:\text.txt");
                for (int i = 2; i <= 10; ++i)
                    DoStatistics(text, i);
            }
            else
            {
                Console.WriteLine("Error Input!");
            }
        }

        static String GetFileString(String path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, (int)fs.Length);
            String text = Encoding.Unicode.GetString(data);
            fs.Close();
            return text;
        }

        delegate void TraverseSubstr(String substr, int index);
        static void Traverse(String str, int len, TraverseSubstr exp)
        {
            for (int i = 0; i < str.Length - len; ++i)
            {
                exp(str.Substring(i, len), i);
            }
        }
        
        // 在str中找到出现次数最多的子串, 长度为len
        static void DoStatistics(String str, int len)
        {
            Dictionary<String, int> dict = new Dictionary<string,int>();
            Traverse(str, len, (substr, index) =>
            {
                if (dict.ContainsKey(substr))
                    dict[substr] += 1;
                else
                    dict[substr] = 1;
            });
            List<KeyValuePair<String, int>> result = dict.OrderByDescending(k => k.Value).ToList();

            for (int i = 0; i < result.Count; ++i)
            {
                if (!AreAllVisibleChar(result[i].Key))
                {
                    result.RemoveAt(i);
                    i--;
                }
            }

            Console.WriteLine("Most often string, length = " + len);
            for (int i = 0; i < 10 && i < result.Count; ++i)
                Console.WriteLine("\"" + result[i].Key + "\"\t\t" + result[i].Value.ToString());
         
        }

        static void DoStatisticsInDictionary(String str, Dictionary<String, int> dict)
        {

        }

        static bool AreAllVisibleChar(String str)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                if (new char[] { ' ', '\r', '\n', '\t' }.Contains(str[i]))
                    return false;
            }
            return true;
        }

        static String HtmlToText(String str)
        {
            return DelHTML.getDelHTML(str);
        }
    }

    public class DelHTML
    {
        public DelHTML()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public static string getDelHTML(string Htmlstring)//将HTML去除
        {
            #region
            //删除脚本

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            //删除HTML

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"-->", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"<!--.*", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            //Htmlstring =System.Text.RegularExpressions. Regex.Replace(Htmlstring,@"<A>.*</A>","");

            //Htmlstring =System.Text.RegularExpressions. Regex.Replace(Htmlstring,@"<[a-zA-Z]*=\.[a-zA-Z]*\?[a-zA-Z]+=\d&\w=%[a-zA-Z]*|[A-Z0-9]","");



            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(amp|#38);", "&", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(lt|#60);", "<", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(gt|#62);", ">", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(nbsp|#160);", " ", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            Htmlstring = System.Text.RegularExpressions.Regex.Replace(Htmlstring, @"&#(\d+);", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);


            Htmlstring.Replace("<", "");

            Htmlstring.Replace(">", "");

            Htmlstring.Replace("\r\n", "");

            //Htmlstring=HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            #endregion


            return Htmlstring;

        }

    }
}

 

