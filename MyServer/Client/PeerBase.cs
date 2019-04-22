using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using MLog;
using MServer.Medium;

namespace MServer
{

    public class PeerBase
    {
        private InitRequest initRequest = new InitRequest();
        public BaseMessage ms = new BaseMessage();
        

        public ApplicationBase Server { get { return initRequest.server; } }
        public Socket Socket { get { return initRequest.socket; } }


        public PeerBase() { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_initRequest"></param>
        public PeerBase(InitRequest _initRequest)
        {
            initRequest = _initRequest;
            
            Start();
        }
        private void Start() {

            initRequest.socket.BeginReceive(ms.data, ms.curIndex, ms.RemainSize, SocketFlags.None, ReceiveCallBack, null);
        }
        private void ReceiveCallBack(IAsyncResult result) {

            if (initRequest.socket == null || initRequest.socket.Connected == false)
            {
                return;
            }
            try
            {
                int count = initRequest.socket.EndReceive(result);
                if (count == 0)
                {
                    Debug.Warring("被动关闭与客户端的连接[CRIP," + initRequest.socket.RemoteEndPoint + "]");
                    initRequest.socket.Shutdown(SocketShutdown.Both);
                    initRequest.socket.Close();
                    initRequest.server.RemovePeerList(this);
                    return;
                }
                ms.ReadMessage(count, OnProcessCallBack, initRequest.socket);

                initRequest.socket.BeginReceive(ms.data, ms.curIndex, ms.RemainSize, SocketFlags.None, ReceiveCallBack, null);

            }
            catch (Exception e)
            {
                #region MyLog
                Debug.Warring("被动关闭客户端[CRIP," + initRequest.socket.RemoteEndPoint.ToString() + "] "+e.ToString());
                #endregion
                initRequest.socket.Close();
                initRequest.server.RemovePeerList(this);
            }
        }

        private void OnProcessCallBack(OperationRequest operationRequest)
        {
            OnOperationRequest(operationRequest);
        }

        public virtual void OnOperationRequest(OperationRequest operationRequest) { }

        public void SendOperationResponse(OperationRequest operationRequest) {

            if (initRequest.socket == null || !initRequest.socket.Connected)
            {
                Console.WriteLine(" 未与客户端建立连接无法传输数据");
                return;
            }
            try
            {
                initRequest.socket.Send(operationRequest.ToBytes());
            }
            catch (Exception)
            {
                Debug.Warring("向客户端传输数据失败");
            }
        } 

        //与客户端断开前
        public virtual void OnDisconnect() { }

        //主动断开连接客户
        public void DisConnect() {

            OnDisconnect();

            if (initRequest.socket != null && initRequest.socket.Connected)
            {

                Debug.Info("主动断开客户端[CRIP," + initRequest.ip.ToString() + "]");
                initRequest.socket.Shutdown(SocketShutdown.Both);
                initRequest.socket.Close();
                //initRequest.server.RemovePeerList(this);
            }
            else {

                Debug.Info("主动断开客户端[CRIP," + initRequest.ip.ToString() + "]");
            }
        }
    }
}
