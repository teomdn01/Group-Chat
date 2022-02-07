using System;
using System.Net;
using System.Net.Sockets;

namespace ServerSide
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Loopback, 1234);
            Server server = new Server(tcpListener);
            server.StartServer();
        }
    }
}
