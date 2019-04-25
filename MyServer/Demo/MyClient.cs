using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLog;
using MServer.Medium;

namespace MServer.Demo
{
    class MyClient : ClientPeer
    {
        //private bool isGetFile = false;
        //private FileStream file = null;

        public MyClient(InitRequest _initRequest) : base(_initRequest) { }

        public override void OnOperationRequest(OperationRequest operationRequest)
        {
            if (operationRequest.Parameters != null)
            {
                Debug.Info("OR[" + operationRequest.OperationCode + "," + operationRequest.Parameters + "]");
            }
            
            switch (operationRequest.OperationCode)
            {
                case SendCode.SEND_FILE_END:
                    break;
                case SendCode.SEND_FILE_DATA:
                    SendRoomMes(operationRequest);
                    break;
                case SendCode.SEND_FILE_HEAD:
                    SendRoomMes(new OperationRequest(SendCode.SEND_FILE_HEAD, operationRequest.Parameters, SendType.File));
                    break;
                case 101:
                    SendOperationResponse(new OperationRequest(10100, CreateRoom(operationRequest.Parameters) ? SendCode.SUCCEED : SendCode.FAILURE));
                    break;
                case 102:
                    SendOperationResponse(new OperationRequest(10200, JoinRoom(operationRequest.Parameters) ? SendCode.SUCCEED : SendCode.FAILURE));
                    break;
                case 103:
                    ExitRoom();
                    break;
                case 104:
                    SendRoomMes(operationRequest);
                    break;
                case 105:
                    SendOperationResponse(new OperationRequest(10500, RoomStatus(operationRequest.Parameters)));
                    break;
                default:
                    break;
            }
        }
        
        public override void OnDisconnect()
        {
            base.OnDisconnect();
        }

        // 101
        public bool CreateRoom(string roomN) {

            if (!RoomSystem.RS.roomMarks.ContainsKey(roomN))
            {
                RoomBed bed = AddComponent<RoomBed>();
                bed.master = this;

                RoomContainer room = AddComponent<RoomContainer>();
                room.InitRoom(roomN, bed);

                RoomSystem.RS.roomMarks.Add(roomN, room);
                return true;
            }
            else {
                return false;
            }
        }

        // 102
        public bool JoinRoom(string roomN)
        {
            if (RoomSystem.RS.roomMarks.ContainsKey(roomN))
            {

                RoomContainer room = RoomSystem.RS.roomMarks[roomN];
                RoomBed bed = AddComponent<RoomBed>();
                bed.master = this;
                return room.JoinRoom(bed);
            }
            return false;
        }

        // 103
        public void ExitRoom()
        {
            RoomBed bed = GetComponent<RoomBed>();
            if (bed.room != null) {
                bed.room.ExitRoom(bed);
            }
            RemoveComponent<RoomBed>();
        }

        // 105
        public void SendRoomMes(OperationRequest operation) {

            RoomBed bed = GetComponent<RoomBed>();
            if (bed != null) {
                bed.SendRoomResponse(operation);
            }
        }
        
        // 106
        public string RoomStatus(string roomN)
        {
            StringBuilder sb = new StringBuilder();
            RoomBed bed = GetComponent<RoomBed>();
            if (bed != null && bed.room != null)
            {
                return bed.room.StatusInfo();
            }
            return String.Empty;
        }
    }
}
