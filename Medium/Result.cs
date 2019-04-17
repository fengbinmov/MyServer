using System;
using System.Collections.Generic;
using System.Text;

public enum ConnectionProtocol : byte
{
    Udp = 0,
    Tcp = 1,
    WebSocket = 4,
    WebSocketSecure = 5
}
namespace MServer.Medium
{

    public class Result
    {
        public int id;
        public string ip;
        public string present;
        public Result() { }
        public Result(int _id, string _ip, string _present)
        {
            id = _id;
            ip = _ip;
            present = _present;
        }
    }
}