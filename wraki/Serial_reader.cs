using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wraki
{
    internal static class Serial_reader
    {
        private static SerialPort port;
        private static List<Tuple<int, int>> list = new();
        private static Stopwatch time;
        private static Window sc;
        private static bool flag;

        public static void Set_port(string com, ref Stopwatch timer)
        {
            flag = false;
            list.Clear();
            sc = new(new Monitor(ref list));
            Application.Current.OpenWindow(sc);
            time = timer;
            port = new(com);
            port.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived);
            port.Open();
        }

        public static void Start()
        {
            flag = true;
        }

        public static void Stop()
        {
            flag = false;
        }

        private static void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!flag) return;
            int t = (int)time.Elapsed.TotalMilliseconds;
            string str = port.ReadLine();
            int p = int.Parse(str);
            Tuple<int, int> tp = new(p, t);
            list.Add(tp);
        }

        public static List<Tuple<int,int>> Get_list()
        {
            return list;
        }
    }
}
