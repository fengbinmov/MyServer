using System;
using System.Threading;
using MLog;
using MServer.Demo;
using System.Collections.Generic;


namespace MServer
{

    class Program
    {
        public static bool ISRUN;

        static List<AppBase> apps;
        static List<Thread> appThread;

        static void Start() {

            ISRUN = true;
            Debug.Init();
            Input.text = "";

            apps = new List<AppBase>();
            appThread = new List<Thread>();
        }

        static void Destroy() {

            Debug.Close();
            apps.Clear();
            appThread.Clear();
            Console.WriteLine("Server Quit");
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            #region 启动程序
            Start();
            #endregion

            #region 启动菜单
        START: Console.WriteLine("1. MyServer");
            string getNum = Console.ReadLine();
            switch (getNum)
            {
                case "1":
                    MyServerApp  myIntnet = new MyServerApp();
                    (myIntnet as MyServerApp).Init(Debug.ServerInfos.serverIP, Debug.ServerInfos.serverProt);
                    apps.Add(myIntnet);
                    break;
                default:
                    goto START;
            }
            #endregion

            #region 运行
            //1.指定程序运行
            for (int i = 0; i < apps.Count; i++)
            {
                Thread threadServer = new Thread(apps[i].OnEnable);
                appThread.Add(threadServer);

                threadServer.Start();
            }

            //2启动输入监听与主程轮训
            while (ISRUN)
            {
                Input.text = Console.ReadLine();
            }

            //3.执行指定程序的结束响应
            for (int i = 0; i < apps.Count; i++) {
                apps[i].enable = false;
            }
            Console.ReadKey();


            #endregion

            #region 关闭响应
            Destroy();
            #endregion
        }
    }
}
