using MServer.Demo;
using System.Collections.Generic;
using MLog;
using System;

namespace MServer
{
    public class InputController
    {
        public static InputController Ins;

        public MyServer server;

        public InputController(MyServer _server) { server = _server; Ins = this; }

        public bool GetLog = true;

        public bool Controller(string code) {


            if (code.Equals(string.Empty)) return true;

            string order = code.ToLower();

            if (GetLog && !order.Equals("l"))
            {
                Debug.Order( "l       指令进入输入状态\n");
                return true;
            }
            switch (order)
            {
                case "dis connect":
                    server.DisAllConnectClient();
                    return true;
                case "dis listen":
                    server.DisListen();
                    return true;
                case "stop server":
                    server.DisAllConnectClient();
                    server.DisListen();
                    return true;
                case "start server":
                    server.StartServer();
                    return true;
                case "s":
                    server.SendAllClient(new Medium.OperationRequest(1, DateTime.Now.ToString("hh:mm:ss ms")));
                    return true;
                case "status":
                    showStatus();
                    return true;
                case "l":
                    MS_ULOG();
                    return true;
                case "lo":
                    MS_OLOG();
                    return true;
                case "q":
                    server.DisAllConnectClient();
                    server.DisListen();
                    return false;
                default:
                    return true;
            }
        }



        void showStatus() {

            Debug.Order("查看连接状态");
            Debug.Info("[ServerIp,"+server.ip+"][serverProt,"+server.prot+"]");
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


        void MS_ULOG() {
            GetLog = false;
            Debug.Order("启动输入窗");

        }

        void MS_OLOG()
        {
            GetLog = true;
            Debug.Order("关闭输入窗");
        }

    }
}
