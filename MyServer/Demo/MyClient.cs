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
        public MyClient(InitRequest _initRequest) : base(_initRequest)
        {
        }
        public override void OnOperationRequest(OperationRequest operationRequest)
        {
            Debug.Info(operationRequest.OperationCode + " " + operationRequest.Parameters);
        }
        public override void OnDisconnect()
        {

        }
    }
}
