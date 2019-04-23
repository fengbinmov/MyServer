using MServer.Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer
{
    public static class Input
    {
        public static string text = null;

        private static List<string> commStr = new List<string> { "cd", "s", "t" };
        private static string commandText = null;
        public static string ComText { get { string t = commandText; commandText = null; return t; } }

        public static bool Equals(string ms)
        {

            if (text == ms)
            {
                text = null;
                return true;
            }
            return false;
        }

        public static string InputText
        {
            get
            {
                return text;
            }
        }

        public static bool GetCommand(string comm)
        {

            if (text == null) return false;

            string[] comms = text.Split(new char[] { ' ' }, 2);
            if (comms.Length == 1 || !commStr.Contains(comms[0]))
            {
                commandText = null;
                return false;
            }
            bool isGet = false;
            if (comms[0] == comm)
                switch (comms[0])
                {
                    case "cd":
                        commandText = GetRealyComm(comms[1]);
                        isGet = true;
                        break;
                    case "s":
                        commandText = comms[1];
                        isGet = true;
                        break;
                    case "t":
                        commandText = comms[1];
                        isGet = true;
                        break;
                    default:
                        break;
                }
            if (isGet) text = "";
            return isGet;
        }
        static string GetRealyComm(string msss)
        {

            string[] ms = msss.Split('/');
            List<string> comm = new List<string>();
            List<string> commFins = new List<string>();
            List<int> delete = new List<int>();

            for (int i = 0; i < ms.Length; i++)
            {
                comm.Add(ms[i]);
            }
            string listT = "";

            for (int i = 0; i < comm.Count; i++)
            {
                if (comm[i] == "..")
                {
                    if (!delete.Contains(i))
                        delete.Add(i);

                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (comm[j] != ".." && !delete.Contains(j))
                        {
                            delete.Add(j);
                            break;
                        }
                    }
                }
                else if (comm[i] == "")
                {
                    if (!delete.Contains(i))
                        delete.Add(i);
                }
            }
            for (int i = 0; i < comm.Count; i++)
            {
                bool havadd = true;
                for (int k = 0; k < delete.Count; k++)
                {
                    if (i == delete[k])
                    {
                        havadd = false;
                        break;
                    }
                }
                if (havadd)
                {
                    commFins.Add(comm[i]);
                }
            }
            for (int i = 0; i < commFins.Count; i++)
            {
                listT += "/" + commFins[i];
            }

            return listT;
        }
    }
}
