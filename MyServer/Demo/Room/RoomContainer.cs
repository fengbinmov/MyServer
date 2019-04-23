using System;
using System.Collections.Generic;
using MServer.Medium;

namespace MServer.Demo
{
    class RoomContainer : MCommponent
    {
        //room master index = 0;
        private List<ClientPeer> roomPeers;
        
        public RoomContainer():base() { }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDestroy()
        {
            CommponentSystem.CS.RemoveCommponent(mName);

            for (int i = 0; i < roomPeers.Count; i++)
            {
                roomPeers[i].RemoveCommponent(mName);
            }
        }

        public bool IsMaster(ClientPeer _peer) {

            if (roomPeers.Count <= 0 || _peer == null)
                return false;
            return roomPeers.IndexOf(_peer) == 0;
        }
        
        public void InitRoom(string _name, ClientPeer _peer) {
            mName = _name;
            roomPeers = new List<ClientPeer>() {
                _peer
            };
        }

        public bool JoinRoom(ClientPeer _peer) {

            if (roomPeers.Count > 0)
            {
                roomPeers.Add(_peer);
                return false;
            }
            return false;
        }

        public void ExitRoom(ClientPeer _peer) {

            if (roomPeers.IndexOf(_peer) > -1) {
                roomPeers.Remove(_peer);
            }
        }

        public void Send(ClientPeer _peer, OperationRequest operation) {

            for (int i = 0; i < roomPeers.Count; i++)
            {
                if (roomPeers[i] != _peer) {

                    roomPeers[i].SendOperationResponse(operation);
                }
            }
        }

        public void Send(ClientPeer _peer, byte[] _data)
        {
            for (int i = 0; i < roomPeers.Count; i++)
            {
                if (roomPeers[i] != _peer)
                {
                    roomPeers[i].Send(_data);
                }
            }
        }

    }
}
