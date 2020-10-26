using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace LogSys
{
    public delegate void ProcessCompleteDel(Headers header, ErrorCodes errorCode);

    //We will pass this into process and cast it back into LoginArgs for ease.
    internal struct LoginArgs
    {
        public string Username;
        public string Password;
        public Headers Header;
        public ProcessCompleteDel Callback;

        public LoginArgs(Headers header, string username, string password, ProcessCompleteDel callback)
        {
            Username = username;
            Password = password;
            Header = header;
            Callback = callback;
        }
    }

    public class LoginClient
    {
        //Our receive buffer
        private byte[] buffer = new byte[512];

        public LoginClient() { }

        public void Login(string username, string password, ProcessCompleteDel callback)
        {
            //Set the args and header.
            LoginArgs args = new LoginArgs(Headers.Login, username, password, callback);
            //Queue the thread
            ThreadPool.QueueUserWorkItem(new WaitCallback(process), args);
        }

        public void Register(string username, string password, ProcessCompleteDel callback)
        {
            //Set the args and header.
            LoginArgs args = new LoginArgs(Headers.Register, username, password, callback);
            //Queue the thread
            ThreadPool.QueueUserWorkItem(new WaitCallback(process), args);
        }

        private void process(object o)
        {
            LoginArgs args = (LoginArgs)o; //Get the struct from the 

            Socket sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //Connect to the server.
                sck.Connect(ServerInfo.Server, ServerInfo.Port);

                //Create our packet and grab the bytes
                PacketWriter pw = new PacketWriter();
                pw.Write((ushort)args.Header);
                pw.Write(args.Username);
                pw.Write(args.Password);

                byte[] data = pw.GetBytes();

                //Send our data
                sck.Send(data, 0, data.Length, SocketFlags.None);

                sck.ReceiveTimeout = 8000;

                //Receive the response
                sck.Receive(buffer, 0, buffer.Length, SocketFlags.None);

                //Create our packet reader and get the header and error code.
                PacketReader pr = new PacketReader(buffer);

                Headers header = (Headers)pr.ReadUInt16();
                ErrorCodes error = (ErrorCodes)pr.ReadUInt16();

                //Call our callback
                args.Callback(header, error);
            }
            catch
            {
                //If something went wrong, use the header we sent to the server, but use only the Error of ErrorCodes
                args.Callback(args.Header, ErrorCodes.Error);
            }
            finally
            {
                //After everything is complete, close the socket.
                sck.Close();
            }
        }
    }
}
