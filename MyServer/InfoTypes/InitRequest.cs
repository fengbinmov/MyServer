using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using MServer;

public sealed class InitRequest
{
    public Socket socket;
    public EndPoint ip;     //CRIP
    public ApplicationBase server;

    public InitRequest() { }
    public InitRequest(Socket _socket, EndPoint _endPoint,ApplicationBase _server) {
        socket = _socket;
        ip = _endPoint;
        server = _server;
    }
}
