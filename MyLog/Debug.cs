using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using System.Collections.Generic;

namespace MLog
{
    /// <summary>
    /// Log 信息输出类
    /// 
    /// 初始化时,会解析软件根目录下的[log4net.config]文件
    /// 主要目的是：
    ///     读取Log文件存放目录
    ///     读取Log信息存入格式
    /// </summary>
    public static class Debug
    {
        public struct ServerInfo
        {
            public string serverIP;
            public int serverProt;
        };
        static string logFileSavePath = string.Empty;   //Log文件存放目录
        static string curLogFileName;                   //当前Log文件名
        static string recordFormat;                     //Log信息存入格式
        public static bool consoleLogPut;               //控制台也输出Log, false 其信息会一直保存在Log文件中只是控制台不输出

        static ulong count = 1;                         //当前程序启动后的第几项log
        static bool canLog = false;                     //是否允许记录Log
        static FileStream stream;                       //文件流 缓存
        static byte[] dataBytes = null;                 //输入字符流 缓存

        public delegate void PrintMLog(string str);
        public static PrintMLog debugLog;
        public static ServerInfo ServerInfos;

        private readonly static object _syncRoot = new object();

        /// <summary>
        ///MyLog 的初始化
        /// </summary>
        public static void Init()
        {

            if (canLog)
            {
                Warring("Log 记录程序已经启动,不需要再次启动.");
                return;
            }
            if (debugLog == null)
            {
                debugLog = Console.WriteLine;
            }

            string filePath = AppDomain.CurrentDomain.BaseDirectory + "log4net.config";

            if (File.Exists(filePath))
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);

                XmlNodeList targets = xmlDoc.GetElementsByTagName("configPath");
                logFileSavePath = targets[0].InnerText;
                XmlNodeList record = xmlDoc.GetElementsByTagName("recordFormat");
                recordFormat = record[0].InnerText;
                XmlNodeList consLog = xmlDoc.GetElementsByTagName("consoleLog");
                consoleLogPut = consLog[0].InnerText.Contains("1");
                XmlNodeList serIP = xmlDoc.GetElementsByTagName("serverIP");
                ServerInfos.serverIP = serIP[0].InnerText;
                XmlNodeList serProt = xmlDoc.GetElementsByTagName("serverProt");
                ServerInfos.serverProt = int.Parse(serProt[0].InnerText);


                xmlDoc.Save(filePath);

                curLogFileName = string.Format("{0}{1}", DateTime.Now.ToString("yyyy-MM-dd"), ".log");
                if (!Directory.Exists(logFileSavePath))
                {

                    debugLog("LogSaveFile 存放目录未找到！！！[" + logFileSavePath + "]");
                    return;
                }
                string pullPath = Path.Combine(logFileSavePath, curLogFileName);
                if (!File.Exists(pullPath))
                {
                    //File.Create(pullPath).Close();
                    stream = File.Create(pullPath);

                }
                else {
                    stream = new FileStream(pullPath, FileMode.Append);
                }
                canLog = true;
                Info("Log 记录程序已经启动");
                //Console.WriteLine("InnerText= [" + logFileSavePath + "]");
                //Console.WriteLine("InnerText= [" + recordFormat+"]");

            }
            else
            {
                debugLog("LogFile 文件未找到！！！[" + filePath + "]");
            }
        }

        /// <summary>
        /// MyLog 的关闭
        /// </summary>
        public static void Close() {
            stream.Close();
        }
       
        /// <summary>
        /// 输出 Info 类型的Log 信息到本地记录文件中
        /// </summary>
        /// <param name="mess">将要存储到Log文件中的信息</param>
        public static void Info(string mess, byte _puter = 0)
        {
            Log(string.Format("{0}{1}", "INFO ", mess), _puter);
        }

        /// <summary>
        /// 输出 Error 类型的Log 信息到本地记录文件中
        /// </summary>
        /// <param name="mess">将要存储到Log文件中的信息</param>
        public static void Error(string mess, byte _puter = 0)
        {
            Log(string.Format("{0}{1}", "ERROR ", mess), _puter);
        }

        /// <summary>
        /// 输出 Warring 类型的Log 信息到本地记录文件中
        /// </summary>
        /// <param name="mess">将要存储到Log文件中的信息</param>
        public static void Warring(string mess, byte _puter = 0)
        {
            Log(string.Format("{0}{1}", "WARRING ", mess), _puter);
        }

        /// <summary>
        /// 输出 Order 类型的Log 信息到本地记录文件中
        /// </summary>
        /// <param name="mess">将要存储到Log文件中的信息</param>
        public static void Order(string mess, byte _puter = 0)
        {
            Log(string.Format("{0}{1}", "ORDER ", mess), _puter);
        }

        static void Log(string mess, byte _puter)
        {

            lock (_syncRoot)
            {

                MakeANewFileName();

                try
                {
                    //stream = new FileStream(Path.Combine(logFileSavePath, curLogFileName), FileMode.Append);
                    dataBytes = FormatString(mess);
                    stream.Write(dataBytes, 0, dataBytes.Length);
                    //stream.Close();
                    if (consoleLogPut) debugLog(mess);
                }
                catch (Exception e)
                {
                    stream.Close();

                    string pullPath = Path.Combine(logFileSavePath, "MLogLog.log");
                    FileStream fileStream;
                    if (!File.Exists(pullPath))
                    {
                        fileStream = File.Create(pullPath);
                    }
                    else
                    {
                        fileStream = new FileStream(pullPath, FileMode.Append);
                    }
                    byte[] datasT = FormatString("ERROR MyLog出现异常![" + e + "]");
                    fileStream.Write(datasT, 0, datasT.Length);
                    fileStream.Close();
                    if (consoleLogPut) debugLog(mess);
                }
            }
        }

        static byte[] FormatString(string mess)
        {

            StringBuilder builder = new StringBuilder(recordFormat);
            builder.Replace("%t", DateTime.Now.ToString("dd HH:mm:ss,ms"));
            builder.Replace("%me", mess);
            builder.Replace("%c", string.Format("[{0}]", (count++).ToString()));
            builder.Append("\r\n");

            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        /// <summary>
        /// 当一天过去时 更新当前文件明，即新建了一个新文件来存放Log 信息
        /// </summary>
        static void MakeANewFileName()
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd");

            string day = curLogFileName.Replace(".log", "");

            if (now != day)
            {
                Console.WriteLine("[" + now + "] [" + day + "]");
                curLogFileName = string.Format("{0}{1}", DateTime.Now.ToString("yyyy-MM-dd"), ".log");

                string pullPath = Path.Combine(logFileSavePath, curLogFileName);
                if (!File.Exists(pullPath))
                {
                    File.Create(pullPath).Close();
                }
            }
        }
    }
}
