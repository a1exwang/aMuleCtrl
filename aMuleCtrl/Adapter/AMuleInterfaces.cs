using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace aMuleCtrl
{
    class AMuleInterfaces
    {
        static int SERVER_PORT = 23456;
        static int RESULTS_PORT = 23457;

        public delegate void OnReceiveAmuleMsg(String op, String data);
        private AMuleInterfaces(OnReceiveAmuleMsg rcv)
        {
            receiver = rcv;
            StartListen();
        }
        public static void Initialize(OnReceiveAmuleMsg rcv)
        {
            TheInstance = new AMuleInterfaces(rcv);
        }
        public static List<Ed2kLink> GetEd2kLinks(String data)
        {
            List<Ed2kLink> links = new List<Ed2kLink>();
            StringReader reader = new StringReader(data);
            while (true)
            {
                String line = reader.ReadLine();
                Ed2kLink link = Ed2kLink.Parse(line);
                if (link == null) break;
                links.Add(link);
            }
            return links;
        }
        static private AMuleInterfaces TheInstance;
        static public AMuleInterfaces GetInstance()
        {
            return TheInstance;
        }
        public void SendMsg(String op, String data)
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Loopback, SERVER_PORT);
            UdpClient client = new UdpClient();
            String str = op + data;
            byte[] sendData = Encoding.Unicode.GetBytes(str);
            client.Send(sendData, sendData.Length, ep);
            client.Close();
        }

        void StartListen()
        {
            new Thread((obj) =>
            {
                while (true)
                {
                    GetMsg();
                }
            }).Start();
        }

        void GetMsg()
        {
            IPEndPoint ep = null;
            UdpClient client = new UdpClient(RESULTS_PORT, AddressFamily.InterNetwork);
            byte[] data = client.Receive(ref ep);
            String str = Encoding.Unicode.GetString(data);
            receiver(str.Substring(0, 3), str.Substring(3, str.Length - 3));
            client.Close();
        }

        private OnReceiveAmuleMsg receiver;
    }

    
}
