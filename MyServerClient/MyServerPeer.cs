using System;
using System.Net;
using System.Net.Sockets;
using MServer.Medium;

namespace MServer.Client {

    public class MyServerPeer
    {
        private Socket socketClient;          //连接套接字
        private IPEndPoint iPEndPoint;
        private BaseMessage mess = new BaseMessage();       //自身的信息处理中介

        public delegate void Apply(OperationRequest operation);
        public delegate void ApplyD(byte[] operation);
        private Apply OnOperationResponse;                  //网络传输响应方法

        public Socket SocketClient { get { return socketClient; } }
        public bool Connected { get { return socketClient != null && socketClient.Connected; } }
        public Apply DebugApply = null;                     //指定输出Debug信息的方法
        public Apply OnDisconnectApply = null;              //指定在断开连接前启动的方法
        private bool canSend = true;

        private bool isDisConnect = false;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_apply">接受到网络信息后的响应方法,接收一个OperationRequest类型参数</param>
        /// <param name="protocol"></param>
        public MyServerPeer(Apply _apply, string ip,int prot)
        {
            OnOperationResponse = _apply;
            iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), prot);
        }

        /// <summary>
        /// 建立对指定IP地址的连接
        /// </summary>
        /// <param name="_ip">IP地址</param>
        /// <param name="_prot">端口号</param>
        public void Connect()
        {
            mess = new BaseMessage();
            try
            {
                socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketClient.BeginConnect(iPEndPoint, ConnectCallBack, socketClient);
            }
            catch (Exception)
            {
                Console.WriteLine("本地端启动失败");
            }
        }

        private void ConnectCallBack(IAsyncResult result) {

            try
            {
                socketClient.BeginReceive(mess.data, mess.curIndex, mess.RemainSize, SocketFlags.None, ReceiveCallBack, socketClient);

                Console.WriteLine("主动连接服务端[CRIP," + socketClient.RemoteEndPoint.ToString() + "]");
                isDisConnect = false;
            }
            catch (Exception)
            {
                Console.WriteLine("主动连接服务端失败，服务端未响应.请重新连接");
            }
        }

        /// <summary>
        /// 在软件启动时主动连接服务器
        /// </summary>
        private void Start()
        {
            if (socketClient == null || socketClient.Connected == false) return;

            socketClient.BeginReceive(mess.data, mess.curIndex, mess.RemainSize, SocketFlags.None, ReceiveCallBack, null);
        }

        /// <summary>
        /// 得到消息后的响应方法
        /// </summary>
        private void ReceiveCallBack(IAsyncResult result)
        {
            try
            {
                int count = socketClient.EndReceive(result);
                if (count == 0) {

                    if (!isDisConnect)
                        DebugLog("被动关闭与服务端的连接[CRIP," + socketClient.RemoteEndPoint + "]");
                    socketClient.Shutdown(SocketShutdown.Both);
                    socketClient.Close();
                    return;
                }
                if (mess.data[0] == (byte)SendType.File)
                {
                    canSend = false;
                }
                mess.ReadMessage(count, OnProcessDataCallBackS, socketClient);
                canSend = true;

                socketClient.BeginReceive(mess.data, mess.curIndex, mess.RemainSize, SocketFlags.None, ReceiveCallBack, null);
            }
            catch (Exception)
            {
                if (!isDisConnect)
                    DebugLog("被动关闭客户端的连接 ");
                socketClient.Close();
            }
        }
        
        /// <summary>
        /// 接受来自服务器的信息,交给RequestManager处理
        /// </summary>
        private void OnProcessDataCallBackS(OperationRequest operationRequest)
        {
            if (OnOperationResponse != null)
                OnOperationResponse.Invoke(operationRequest);
        }

        /// <summary>
        /// 发送消息到服务器
        /// </summary>
        /// <param name="operationRequest"> 操作信息，用来记录此次传送的指令与信息</param>
        public void SendRequest(OperationRequest operationRequest)
        {
            Send(operationRequest.operationData);
        }

        /// <summary>
        /// 发送字节到服务器
        /// </summary>
        /// <param name="operationRequest"> 操作信息，用来记录此次传送的指令与信息</param>
        public void Send(byte[] bytes) {

            if (socketClient == null || !socketClient.Connected)
            {
                DebugLog("未与服务器建立连接无法传输数据");
                return;
            }
            if (!canSend)
            {
                Console.WriteLine("已禁止了传输数据");
                return;
            }
            try
            {
                socketClient.Send(bytes);
            }
            catch (Exception)
            {
                Console.WriteLine("向服务端端传输数据失败");
            }
        }

        /// <summary>
        /// 关闭与服务器之间的连接
        /// </summary>
        public void DisConnect()
        {
            if (OnDisconnectApply != null) OnDisconnectApply.Invoke(new OperationRequest(0, string.Empty));

            if (socketClient != null && socketClient.Connected)
            {
                DebugLog("主动断开服务端[LRIP," + socketClient.RemoteEndPoint.ToString() + "]");

                socketClient.Shutdown(SocketShutdown.Both);
                socketClient.Close();
                isDisConnect = true;
            }
            else
            {
                DebugLog("主动断开服务端");
                socketClient.Close();
                isDisConnect = true;
            }
        }

        private void DebugLog(string ms) {
            DebugApply?.Invoke(new OperationRequest(0, ms));
        }
    }
}