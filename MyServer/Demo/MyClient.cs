using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLog;
using MServer.Medium;

namespace MServer.Demo
{
    class MyClient : ClientPeer
    {
        public string mRoomN = "";

        public MyClient(InitRequest _initRequest) : base(_initRequest){}

        public override void OnOperationRequest(OperationRequest operationRequest)
        {
            Console.WriteLine(operationRequest.OperationCode + " " + operationRequest.Parameters);
            switch (operationRequest.OperationCode)
            {
                case 101:
                    CreateRoom(operationRequest.Parameters);
                    break;
                case 102:
                    JoinRoom(operationRequest.Parameters);
                    break;
                case 103:
                    ExitRoom(operationRequest.Parameters);
                    break;
                case 104:
                    DestoryRoom(operationRequest.Parameters);
                    break;
                case 105:
                    SendRoomMes(operationRequest);
                    break;
                case 106:
                    SendRoomMes(new byte[] { });
                    break;
                default:
                    break;
            }
        }

        public override void OnDisconnect()
        {
        }

        // 101
        public bool CreateRoom(string roomN) {

            RoomContainer room = new RoomContainer();
            room.InitRoom(roomN, this);

            if (CommponentSystem.CS.AddCommponent(room)) {
                mRoomN = roomN;
                return AddCommponent(room);
            }

            return false;
        }

        // 102
        public bool JoinRoom(string roomN)
        {

            mRoomN = roomN;
            RoomContainer room = CommponentSystem.CS.GetCommponent(roomN) as RoomContainer;

            if (room != null) {
                if(room.JoinRoom(this))
                    return AddCommponent(room);
            }
            return false;
        }

        // 103
        public void ExitRoom(string roomN)
        {
            mRoomN = "";
            RoomContainer room = GetCommponent(roomN) as RoomContainer;
            if (room == null) return;
            room.ExitRoom(this);
            RemoveCommponent(roomN);
        }

        // 104
        public void DestoryRoom(string roomN)
        {
            RoomContainer room = GetCommponent(roomN) as RoomContainer;
            if (room.IsMaster(this)) {

                room.OnDestroy();
            }
            mRoomN = "";
        }

        // 105
        public void SendRoomMes(OperationRequest operation) {

            (GetCommponent(mRoomN) as RoomContainer).Send(this, operation);
        }

        // 106
        public void SendRoomMes(byte[] _data)
        {
            (GetCommponent(mRoomN) as RoomContainer).Send(this, _data);
        }
    }
}
