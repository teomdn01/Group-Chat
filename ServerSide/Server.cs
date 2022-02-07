using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    class Server
    {
        private TcpListener _listener;

        /// <summary>
        /// Server constructor
        /// </summary>
        /// <param name="listener">provides server-side Socket connection</param>
        public Server(TcpListener listener)
        {
            this._listener = listener;
        }

        /// <summary>
        /// The TcpListener listens for connections to be made from TcpClients
        /// AcceptTcpClient is a blocking method => the program waits until a connection is made
        /// Then, a client handler listens for messages received from the ClientSide. As this is also a blocking method,
        /// We run it on a separate thread so that we can continue and wait for new TcpClients that want to connect.
        /// </summary>
        public void StartServer()
        {
            _listener.Start();
            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                Console.WriteLine("A new client has connected!");
                ClientHandler clientHandler = new ClientHandler(client);
                Task.Run(() =>
                {
                    clientHandler.ListenToMessage();
                });
            }
        }
    }
}
