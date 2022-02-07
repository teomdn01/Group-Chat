using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    /// <summary>
    /// ClientHandler class helps us broadcast the messages/signals between the clients.
    /// </summary>
    class ClientHandler
    {
        public static List<ClientHandler> ClientHandlers { get; set; } = new List<ClientHandler>();  //used to broadcast messages to the other clients
        public TcpClient TcpClient { get; }
        public NetworkStream NetworkStream { get; set; }
        public string ClientUsername { get; set; }

        public ServerPacketReader Reader { get; set; }

        /// <summary>
        /// Constructor of the ClientHandler class. Sets up the communication streams and adds the current instance to the clientHandlers list.
        /// </summary>
        /// <param name="client">the socket on which the respective client connected.</param>
        public ClientHandler(TcpClient client)
        {
            try
            {
                TcpClient = client;
                NetworkStream = client.GetStream();
                Reader = new ServerPacketReader(NetworkStream);

                ClientUsername = Reader.ReadMessage();

                ClientHandlers.Add(this);
                BroadcastMessage("SERVER: " + ClientUsername + " has entered the chat");

            }
            catch (ArgumentException e)
            {
                Console.WriteLine("ClientHandler ArgumentException: " + e.Message);
                CloseEverything(TcpClient, Reader);
            }
            catch (IOException e)
            {
                Console.WriteLine("ClientHandler IOException" + e.Message);
                CloseEverything(TcpClient, Reader);
            }

        }

        /// <summary>
        /// Sends the message received from one client to all the other clients
        /// </summary>
        /// <param name="messageToSend">message to be sent to the other clients</param>
        public void BroadcastMessage(string messageToSend)
        {
            foreach (ClientHandler clientHandler in ClientHandlers)
            {
                try
                {
                    if (clientHandler.ClientUsername != ClientUsername)
                    {
                        //Writer.WriteLine(messageToSend);
                        //flush the stream so the client gets the message
                        //byte[] buffer = new byte[TcpClient.ReceiveBufferSize];
                        //byte[] bytesToSend = Encoding.ASCII.GetBytes(messageToSend);
                        //NetworkStream.Write(buffer, 0, bytesToSend.Length);
                        var Writer = new ServerPacketWriter();
                        Writer.WriteMessage(messageToSend);
                        clientHandler.TcpClient.Client.Send(Writer.GetPacketBytes());
                    }
                }
                catch (IOException e)
                {
                    CloseEverything(TcpClient, Reader);
                }
            }
        }

        /// <summary>
        /// Listens for messages from the client, but works on a separate thread, because
        /// listening for a new message will block the rest of the application
        /// </summary>
        public void ListenToMessage()
        {
            while (TcpClient != null && TcpClient.Client != null && TcpClient.Client.Connected)
            {
                //blocks the method until receives the message
                try
                {
                    string messageFromClient = Reader.ReadMessage();
                    BroadcastMessage(messageFromClient);
                }

                catch (IOException e)
                {
                    CloseEverything(TcpClient, Reader);
                    break;
                }

            }
        }

        /// <summary>
        /// Removes the current reference from the clientHandler list
        /// </summary>
        public void RemoveClientHandler()
        {
            ClientHandlers.Remove(this);
            Console.WriteLine("Client with the username '" + this.ClientUsername + "' has left the chat!");
            BroadcastMessage("SERVER: " + ClientUsername + " has left the chat!");
        }

        /// <summary>
        ///  Closing the connection and the reading stream
        /// </summary>
        /// <param name="tcpClient">the socket on which the client is connected</param>
        /// <param name="reader">reading channel</param>
        public void CloseEverything(TcpClient tcpClient, ServerPacketReader reader)
        {
            RemoveClientHandler();
            if (reader != null)
            {
                reader.Close();
            }
            if (tcpClient.Client != null)
            {
                tcpClient.Client.Close();
            }
            if (tcpClient != null)
            {
                tcpClient.Close();
            }
        }
    }
}
