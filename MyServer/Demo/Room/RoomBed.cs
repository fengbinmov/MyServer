using MServer.Medium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer.Demo
{
    class RoomBed :MComponent
    {
        public RoomContainer room;
        public ClientPeer master;

        public RoomBed() :base() { }

        public void SendRoomResponse(OperationRequest operation)
        {
            if (room != null)
            {
                room.SendRoomResponse(this,operation);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            master = null;
            room = null;
        }
    }
}
