using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace ViewIPsInTheHub
{
    class Program
    {
        static List<string> ipList = new List<string>();
        static List<string> newIpList = new List<string>();
        static List<string> oldIpList = new List<string>();

        static int count = 0;
        static void Main(string[] args)
        {
            Console.WriteLine("Do not connect smartbox. Press any key to continue.");
            Console.ReadKey();
            Console.WriteLine("Start searching, please wait...");
            GetIP(ref oldIpList);

            Console.WriteLine("Connect smartbox. Press any key to continue.");
            Console.ReadKey();
            Console.WriteLine("Start searching, please wait...");
            GetIP(ref newIpList);

            foreach (var ip in oldIpList)
            {
                int index = newIpList.IndexOf(ip);
                if (index != -1)
                {
                    newIpList.RemoveAt(index);
                }
            }
            Console.WriteLine("Smartbox ip is:");
            foreach (var ip in newIpList)
            {
                Console.WriteLine(ip);
            }
        }

        public static void SearchAllIps()
        {
            string input = "y";
            while (input.Equals("y") || input.Equals("Y"))
            {
                Console.WriteLine("Start searching, please wait...");
                GetIP(ref ipList);
                Console.WriteLine("Please input y or Y to continue, else will quit.");
                input = Console.ReadLine();
            }
        }

        public static void GetIP(ref List<string> ipList)
        {
            //获取本地机器名 
            string _myHostName = Dns.GetHostName();
            //获取本机IP 
            string _myHostIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(a => a.AddressFamily.ToString().Equals("InterNetwork")).ToString();
            //MessageBox.Show(_myHostIP);
            //截取IP网段
            string ipDuan = _myHostIP.Remove(_myHostIP.LastIndexOf('.'));
            //MessageBox.Show(ipDuan);
            //枚举网段计算机
            Ping myPing = new Ping();
            string data = "";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            for (int i = 1; i <= 255; i++)
            {
                string pingIP = ipDuan + "." + i.ToString();
                PingReply pingReply = myPing.Send(pingIP, 120, buffer);
                string text;
                if (pingReply.Status == IPStatus.Success)
                {
                    ipList.Add(pingIP);
                    text = "答复的主机地址：" + pingReply.Address.ToString() + "\n";
                    text += "往返时间：" + pingReply.RoundtripTime.ToString() + "\n";
                    text += "生存时间：" + pingReply.Options.Ttl.ToString() + "\n";
                    text += "是否控制数据包的分段：" + pingReply.Options.DontFragment.ToString() + "\n";
                    text += "缓冲区大小：" + pingReply.Buffer.Length.ToString() + "\n";
                }
                else
                {
                    text = pingIP + "不在线";
                }

                //Console.WriteLine(text);
            }

            foreach(var ip in ipList)
            {
                Console.WriteLine(ip);
            }
        }

        private static void EnumComputers()
        {
            try
            {
                count = 0;
                for (int i = 1; i <= 255; i++)
                {
                    Ping myPing;
                    myPing = new Ping();
                    myPing.PingCompleted += new PingCompletedEventHandler(_myPing_PingCompleted);

                    string _myHostIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(a => a.AddressFamily.ToString().Equals("InterNetwork")).ToString();
                    string ipDuan = _myHostIP.Remove(_myHostIP.LastIndexOf('.'));

                    string pingIP = ipDuan + "." + i.ToString();
                    myPing.SendAsync(pingIP, 2000, null);
                }
            }
            catch
            {
                ;
            }
        }

        private static void _myPing_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            if (e.Reply.Status == IPStatus.Success)
            {
                ipList.Add(e.Reply.Address.ToString());
            }

            count++;

            if (count >= 255)
            {
                Thread t1 = new Thread(new ParameterizedThreadStart(TestMethod));
                t1.IsBackground = true;
                t1.Start();
            }
        }

        private static void TestMethod(object obj)
        {
            foreach (var ip in ipList)
            {
                Console.WriteLine(ip);
            }
        }
    }
}
