using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Our_Login_Server
{
    public delegate void SocketAcceptedEventHandler(object sender, Socket e);
    class Listener
    {
        //This will be our listener socket.
        private Socket socket;

        //A property so we know if our listener is running or not.
        public bool Running
        {
            get;
            private set;
        }

        //We will call this event once we accept a connection.
        public event SocketAcceptedEventHandler Accepted;

        public Listener()
        {
        }

        public void Start(int port)
        {
            if (socket != null)
                return;
            Running = true;

            //We create our socket.
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            /*We call bind our socket to IPAddress.Any to accept any connection on our desired port.
             * MSDN Socket.Bind definition
             * http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.bind.aspx
             */
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            //We place the socket into a listening state.
            socket.Listen(100);
            //Now we wait for connections.
            socket.BeginAccept(callback, null);
        }

        public void Stop()
        {
            if (socket == null) //If the socket is already closed. (We will set the socket to null after closing)
                return;
            Running = false;
            socket.Close();
        }

        private void callback(IAsyncResult ar)
        {
            try
            {
                Socket acc = socket.EndAccept(ar); //We call EndAccept to get the socket.
                /*
                 * MSDN Socket.BeginAccept
                 * http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.beginaccept.aspx
                 * MSDN Socket.EndAccept
                 * http://msdn.microsoft.com/en-us/library/system.net.sockets.socket.endaccept.aspx
                 */

                if (Accepted != null) //If our event is registered. (It will throw an exception if we try to call it without-
                    //It being registered.
                {
                    Accepted(this, acc);
                }
            }
            catch
            {
            }

            if (Running) //If the listener is still running, keep going.
                socket.BeginAccept(callback, null);
        }
    }
}
