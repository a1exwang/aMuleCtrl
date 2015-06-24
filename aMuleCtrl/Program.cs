using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Data.SQLite;

namespace aMuleCtrl
{
    class Program
    {
        // btcherry
        static void Main(string[] args)
        {
            Config.Initialize(args);
            Config config = Config.GetInstance();

            InnerCommandProcessor iproc = new InnerCommandProcessor();
            OutterCommandProcessor oproc = new OutterCommandProcessor();

            if(!config.IsSilent())
                InnerCommandProcessor.HelpMsg();

            // 主循环
            while (true)
            {
                Console.Write("-->");
                String line = Console.ReadLine();
                
                // including invalid commands
                if (iproc.Process(line))
                    continue;

                String op = line.Substring(0, 3), data = "";
                if(line.Length > 3)
                    data = line.Substring(4, line.Length - 4);

                oproc.Process(op, data);
            }
        }
    }
}
