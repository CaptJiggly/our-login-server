using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
namespace Our_Login_Server
{
    class Client
    {
        //This will hold the connected socket
        private Socket socket;

        //This will hold our received data.
        private byte[] buffer = new byte[512];

        public Client(Socket sck)
        {
            socket = sck;
            socket.ReceiveTimeout = 5000; //We will be using a timeout, but its not needed.
        }

        public byte[] Receive()
        {
            SocketError error; //If something goes wrong, it will be stored here instead of an exception being thrown

            int res = socket.Receive(buffer, 0, buffer.Length, SocketFlags.None, out error);

            //If its not success, then something went wrong. (Such as disconnection, etc)
            if (error != SocketError.Success)
            {
                return null;
            }

            return buffer; //Return our buffer we used to receive
        }

        public void Send(byte[] data)
        {
            SocketError error;
            socket.Send(data, 0, data.Length, SocketFlags.None, out error);
        }

        public void Close()
        {
            socket.Close();
            buffer = null;
        }
    }
}
