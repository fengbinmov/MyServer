using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MServer.Demo
{
    public class MyServer :ApplicationBase
    {
        public MyServer(string _ip, int _port) : base(_ip, _port)
        {
        }

        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new MyClient(initRequest);
        }
    }
}
