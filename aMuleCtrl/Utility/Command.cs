using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace aMuleCtrl
{
    class Command
    {
        /// 
        /// 运行指定命令行 
        /// 
        /// 命令 
        /// 命令行参数 
        /// 写入命令行的确认信息 
        /// 
        public static string Execute(string cmd, string arg, string dir)
        {
            Process p = new Process();
            p.StartInfo.FileName = cmd;
            p.StartInfo.Arguments = arg;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.LoadUserProfile = true;
            p.StartInfo.WorkingDirectory = dir;

            p.Start();
            string msg = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();

            return msg;
        }
    }
}