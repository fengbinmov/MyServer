using MLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer.Demo
{
    class MyServerApp : AppBase
    {
        public string serverIP;
        public int serverProt;
        private bool isInit = false;
        MyServer server;

        public void Init(string ip, int prot) {
            serverIP = ip;
            serverProt = prot;
            isInit = true;
        }
        public override void Start()
        {
            if (!isInit) return;

            Console.WriteLine("MyServer 已启动");
            server = new MyServer(serverIP, serverProt);
            server.StartServer();
        }
        public override void Update()
        {
            string[] ms = Input.text.Split(' ');
            switch (ms[0])
            {
                case "dis connect":
                    server.DisAllConnectClient();
                    Input.text = "";
                    break;
                case "dis listen":
                    server.DisListen();
                    Input.text = "";
                    break;
                case "stop server":
                    server.DisAllConnectClient();
                    server.DisListen();
                    Input.text = "";
                    break;
                case "start server":
                    server.StartServer();
                    Input.text = "";
                    break;
                case "s":
                    server.SendAllClient(new Medium.OperationRequest(1, ms[1]));
                    Input.text = "";
                    break;
                case "status":
                    showStatus();
                    Input.text = "";
                    break;
                case "q":
                    server.DisAllConnectClient();
                    server.DisListen();
                    Program.ISRUN = false;
                    Input.text = "";
                    Console.ReadKey();
                    break;
                default:
                    break;
            }
        }
        public override void Destroy()
        {
            Console.WriteLine("MyServer 已关闭");
        }
        void showStatus()
        {

            Debug.Order("查看连接状态");
            Debug.Info("[ServerIp," + server.ip + "][serverProt," + server.prot + "]");
            List<PeerBase> peers = server.ByServerGetAllPeer();
            if (peers.Count > 0)
            {
                for (int i = 0; i < peers.Count; i++)
                {
                    MyClient myClient = peers[i] as MyClient;
                    Debug.Info(myClient.Socket.RemoteEndPoint.ToString());
                }
            }
            else
            {
                Debug.Order("空数据");
            }
        }
    }
}
