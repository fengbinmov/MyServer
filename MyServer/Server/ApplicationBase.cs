using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using MLog;
using MServer.Medium;

namespace MServer
{
    public class ApplicationBase
    {
        private IPEndPoint iPEndPoint;
        private Socket serverSocket;
        
        private Dictionary<Socket, string> socketDict = new Dictionary<Socket, string>();
        private List<PeerBase> peerList = new List<PeerBase>();
        public List<PeerBase> PeerList { get { return peerList; } }

        private PeerBase currPeer;

        public string ip;
        public int prot;

        public ApplicationBase(string _ip, int _port) {

            iPEndPoint = new IPEndPoint(IPAddress.Parse(_ip),_port);
            ip = _ip;
            prot = _port;

            ComponentSystem.CS = new ComponentSystem();
            ComponentSystem.CS.appServer = this;
            #region MyLog
            Debug.Info("MyServer 初始化:[" + _ip + ":" + _port + "]");
            #endregion
        }

        /// <summary>
        /// 开始启动服务器的监听
        /// </summary>
        private void Start() {

            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                serverSocket.Bind(iPEndPoint);
                serverSocket.Listen(0);

                serverSocket.BeginAccept(AccpetAsyncCallBack, this);
            }
            catch (Exception) {

                Console.WriteLine("服务端启动失败");
            }

            #region MyLog
            Debug.Info("MyServer 已经启动");
            #endregion
        }

        /// <summary>
        /// 异步接受网络信息
        /// </summary>
        private void AccpetAsyncCallBack(IAsyncResult result) {

            try
            {
                Socket socket = serverSocket.EndAccept(result);
                EndPoint ip = (IPEndPoint)socket.RemoteEndPoint;

                currPeer = CreatePeer(new InitRequest(socket, ip, this));

                peerList.Add(currPeer);
                #region MyLog
                Debug.Info("监听到有客户端连接[CRIP," + socket.RemoteEndPoint.ToString() + "]");
                #endregion
                serverSocket.BeginAccept(AccpetAsyncCallBack, this);
            }
            catch (Exception) {
                #region MyLog
                Debug.Info("服务端监听过程终止");
                #endregion
            }
        }

        /// <summary>
        /// 创建一个新的客户端网络监听器
        /// </summary>
        /// <returns>将创建的监听器引用返回</returns>
        protected virtual PeerBase CreatePeer(InitRequest initRequest) {
            
            return new PeerBase(initRequest);
        }

        /// <summary>
        /// 主动断开所有连接客户
        /// </summary>
        public void DisAllConnectClient() {


            if (peerList.Count > 0)
            {
                Console.WriteLine("主动断开所有连接客户[Count,"+ peerList.Count+"]");
                for (int i = 0; i < peerList.Count; i++)
                {
                    peerList[i].DisConnect();
                }
                peerList.Clear();
            }
            else
            {
                Console.WriteLine("主动断开所有连接客户[Count,0]");
            }
            Console.WriteLine("已断开所有连接客户");
        }

        /// <summary>
        /// 关闭服务端监听
        /// </summary>
        public void DisListen() {

            if (serverSocket != null && serverSocket.Connected)
            {
                Console.WriteLine("主动关闭服务端监听[SLIP," + serverSocket.LocalEndPoint.ToString() + "]");
                serverSocket.Shutdown(SocketShutdown.Both);
                serverSocket.Close();
            }
            else
            {
                Console.WriteLine("主动关闭服务端监听");
                serverSocket.Close();
            }
        }
        
        /// <summary>
        /// 停止服务端
        /// </summary>
        public void StopServer()
        {
            DisAllConnectClient();

            DisListen();
        }

        /// <summary>
        /// 开始服务端
        /// </summary>
        public void StartServer() {

            if (serverSocket == null || !serverSocket.Connected)
            {
                Start();
            }
        }

        public void SendAllClient(OperationRequest operation) {

            if (peerList.Count > 0)
            {
                for (int i = 0; i < peerList.Count; i++)
                {
                    peerList[i].SendOperationResponse(operation);
                }
            }
            else {
                Debug.Info("客户端连接数为 0");
            }
        }

        //TODO
        //public void SendAllClient(Byte[] _data)
        //{

        //    if (peerList.Count > 0)
        //    {
        //        for (int i = 0; i < peerList.Count; i++)
        //        {
        //            peerList[i].Send (_data);
        //        }
        //    }
        //    else
        //    {
        //        Debug.Info("客户端连接数为 0");
        //    }
        //}

        /// <summary>
        /// 服务器关闭后调用的方法
        /// </summary>
        protected virtual void TearDown() {

        }

        #region 扩展方法

        /// <summary>
        /// 将断开连接的Peer 从PeerList中移除，并置为NULL
        /// </summary>
        public void RemovePeerList(PeerBase _peer) {

            peerList.Remove(_peer);
        }

        public List<PeerBase> ByServerGetAllPeer() {

            return peerList;
        }

        #endregion
    }
}
