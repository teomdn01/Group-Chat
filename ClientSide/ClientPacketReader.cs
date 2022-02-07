using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientSide
{
    /// <summary>
    /// Packet reader class for reading the bytes received from the client
    /// </summary>
    public class ClientPacketReader : BinaryReader
    {
        private NetworkStream networkStream;
        public ClientPacketReader(NetworkStream networkStream) : base(networkStream)
        {
            this.networkStream = networkStream;
        }

        /// <summary>
        /// Reads the packet content, first - the length of the message, then the message itself
        /// </summary>
        /// <returns>message received from server</returns>
        public string ReadMessage()
        {
            byte[] messageBuffer;
            int length = ReadInt32();
            messageBuffer = new byte[length];
            networkStream.Read(messageBuffer, 0, length);
            string message = Encoding.ASCII.GetString(messageBuffer);
            return message;
        }
    }
}
