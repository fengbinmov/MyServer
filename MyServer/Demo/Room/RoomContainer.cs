using System;
using System.Collections.Generic;
using MServer.Medium;
using System.Text;

namespace MServer.Demo
{
    class RoomContainer : MComponent
    {
        //room master index = 0;
        private List<RoomBed> roomBeds = new List<RoomBed>();

        public RoomContainer():base() { }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            if (RoomSystem.RS.roomMarks.ContainsKey(mName))
            {
                RoomSystem.RS.roomMarks.Remove(mName);
            }
            for (int i = 0; i < roomBeds.Count; i++)
            {
                roomBeds[i].room = null;
            }
            roomBeds.Clear();
        }

        public bool IsMaster(RoomBed bed) {

            if (roomBeds.Count < 1) return false;

            return roomBeds[0] == bed;
        }
        
        public void InitRoom(string _name, RoomBed bed) {

            if (roomBeds.Count == 0) {

                mName = _name;
                bed.room = this;
                roomBeds.Add(bed);
            }
        }

        public bool JoinRoom(RoomBed bed) {

            if (roomBeds.Count > 0)
            {
                bool havself = false;
                for (int i = 0; i < roomBeds.Count; i++)
                {
                    if (roomBeds[i] == bed) {
                        havself = true;
                        break;
                    }
                }
                if (!havself) {

                    bed.room = this;
                    roomBeds.Add(bed);
                    return true;
                }
            }
            return false;
        }

        public void ExitRoom(RoomBed bed)
        {

            if (roomBeds.Count > 0)
            {
                bool havself = false;
                for (int i = 0; i < roomBeds.Count; i++)
                {
                    if (roomBeds[i] == bed)
                    {
                        havself = true;
                        break;
                    }
                }
                if (havself)
                {
                    if (roomBeds[0] == bed && roomBeds.Count > 1)
                    {
                        roomBeds[1].master.AddComponent(bed.master.PopComponent<RoomContainer>());
                    }
                    else if (roomBeds.Count == 1)
                    {
                        bed.master.RemoveComponent<RoomContainer>();
                    }
                    roomBeds.Remove(bed);
                }
            }
        }

        public void SendRoomResponse(RoomBed bed, OperationRequest operation) {

            for (int i = 0; i < roomBeds.Count; i++)
            {
                if (roomBeds[i] != null && roomBeds[i] != bed) {

                    roomBeds[i].master.SendOperationResponse(operation);
                }
            }
        }

        public string StatusInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(roomBeds.Count);
            sb.Append("|");
            sb.Append(mName);
            for (int i = 0; i < roomBeds.Count; i++)
            {
                sb.Append("|");
                if (roomBeds[i] != null && roomBeds[i].master.Socket.Connected)
                {
                    sb.Append(roomBeds[i].master.Socket.RemoteEndPoint.ToString());
                }
            }
            return sb.ToString();
        }
    }
}
