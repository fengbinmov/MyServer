using System;
using System.Collections.Generic;

namespace MServer
{
    public sealed class ComponentSystem
    {
        public static ComponentSystem CS;

        public ApplicationBase appServer;

        public List<T> GetCommponents<T>() where T : MComponent
        {

            List<T> coms = new List<T>();
            
            for (int i = 0; i < appServer.PeerList.Count; i++)
            {
                MComponent com = (appServer.PeerList[i] as ClientPeer).GetComponent<T>();

                if (com != null) coms.Add(com as T);
            }
            return coms;
        }
    }
}
