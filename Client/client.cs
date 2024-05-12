using Modules;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class client
    {
        private static readonly string END = "|#|#|#|";
        private readonly string serverIP;
        private readonly int port;
        private Socket? socket;
        private const int chunkSize = 1024;

        public client(string serverIP, int port = 11000)
        {
            this.serverIP = serverIP;
            this.port = port;
        }

        public bool Connect()
        {
            IPHostEntry iPHostEntry = Dns.GetHostEntry(serverIP);
            IPAddress serverAddress = iPHostEntry.AddressList[0];
            IPEndPoint serverEndPoint = new(serverAddress, port);
            socket = new(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(serverEndPoint);
            return true;
        }

        public void Disconnect()
        {
            if (socket is null)
            {
                throw new InvalidOperationException("Client is not connected to the server.");
            }
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        public async Task Send(object obj)
        {
            if (socket is null)
            {
                throw new InvalidOperationException("Client is not connected to the server.");
            }
            byte[] data = Encoding.UTF8.GetBytes(Jsonifier.ToJson(obj));
            await Task.Run(() =>
            {
                // send 1024 bytes at a time till data is completly sent
                for (int i = 0; i < data.Length; i += chunkSize)
                {
                    int bytesToSend = Math.Min(chunkSize, data.Length - i);
                    socket.Send(data, i, bytesToSend, SocketFlags.None);
                }
                socket.Send(Encoding.UTF8.GetBytes(END));
            });
        }

        public async Task<object> Recieve()
        {
            if (socket is null)
            {
                throw new InvalidOperationException("Client is not connected to the server.");
            }
            byte[] data = new byte[chunkSize];
            StringBuilder stringBuilder = new();
            await Task.Run(() =>
            {
                while (true)
                {
                    int bytesRecieved = socket.Receive(data);
                    string lastRecieved = Encoding.UTF8.GetString(data, 0, bytesRecieved);
                    if (lastRecieved.EndsWith(END))
                    {
                        stringBuilder.Append(lastRecieved[..^END.Length]);
                        break;
                    }
                    stringBuilder.Append(lastRecieved);
                }
            });
            return Jsonifier.ToObject(stringBuilder.ToString());
        }
    }
}
