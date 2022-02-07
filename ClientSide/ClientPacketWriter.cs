using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Packet writer class used to send data to the Server through the MemoryStream
/// </summary>
namespace ClientSide
{
    class ClientPacketWriter
    {
        private MemoryStream memoryStream;

        public ClientPacketWriter()
        {
            this.memoryStream = new MemoryStream();
        }

        /// <summary>
        /// Writes a block of bytes to the current stream. First the length of the message, then the message itself
        /// </summary>
        /// <param name="message">The message to be sent to the server</param>
        public void WriteMessage(string message)
        {
            var length = message.Length;
            memoryStream.Write(BitConverter.GetBytes(length), 0, BitConverter.GetBytes(length).Length);
            memoryStream.Write(Encoding.ASCII.GetBytes(message), 0, Encoding.ASCII.GetBytes(message).Length);
        }

        /// <summary>
        /// Converts the blocks of bytes from the Memory Stream to byte array
        /// </summary>
        /// <returns>byte[] - data </returns>
        public byte[] GetPacketBytes()
        {
            return memoryStream.ToArray();
        }
    }
}
