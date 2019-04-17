using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer
{
    public abstract class ClientPeer : PeerBase
    {

        protected ClientPeer(InitRequest _initRequest):base(_initRequest) { }
    }
}
