using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Our_Login_Server
{
    class Program
    {
        public const int PORT = 1234;

        static void Main(string[] args)
        {
            SQL.Open("UserDB.sdf");
            Listener listener = new Listener();
            listener.Accepted += listener_Accepted;
            listener.Start(PORT);

            System.Diagnostics.Process.GetCurrentProcess().WaitForExit();
        }

        static void listener_Accepted(object sender, System.Net.Sockets.Socket e)
        {
            Client client = new Client(e);
            byte[] data = client.Receive();

            if (data == null)
            {
                client.Close();
                return;
            }

            PacketReader pr = new PacketReader(data);

            Headers header = (Headers)pr.ReadUInt16();

            switch (header)
            {
                case Headers.Login:
                    {
                        ProcessLogin(client, pr.ReadString(), pr.ReadString());
                    }
                    break;
                case Headers.Register:
                    {
                        ProcessRegister(client, pr.ReadString(), pr.ReadString());
                    }
                    break;
            }

            client.Close();
        }

        static void SendPacketWriter(Client client, PacketWriter pw)
        {
            byte[] data = pw.GetBytes();
            client.Send(data);
        }

        static void ProcessRegister(Client client, string username, string password)
        {
            PacketWriter pw = new PacketWriter();

            pw.Write((ushort)Headers.Register);

            if (UserManagement.Exists(username))
            {
                pw.Write((ushort)ErrorCodes.Exists);
                SendPacketWriter(client, pw);
                return;
            }

            if (UserManagement.Register(username, password))
            {
                pw.Write((ushort)ErrorCodes.Success);
            }
            else
            {
                pw.Write((ushort)ErrorCodes.Error);
            }

            SendPacketWriter(client, pw);

        }

        static void ProcessLogin(Client client, string username, string password)
        {
            PacketWriter pw = new PacketWriter();
            pw.Write((ushort)Headers.Login);

            if (UserManagement.Login(username, password))
            {
                pw.Write((ushort)ErrorCodes.Success);
            }
            else
            {
                pw.Write((ushort)ErrorCodes.InvalidLogin);
            }

            SendPacketWriter(client, pw);
        }
    }
}
