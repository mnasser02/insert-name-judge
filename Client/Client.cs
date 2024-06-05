using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using Modules;

namespace Client {
    class ClientApp {
        private static readonly string END_TOKEN = "!##<|EOF|>";
        private readonly IPAddress serverIp;
        private readonly int port;
        private Socket? socket;
        private const int CHUNK_SIZE = 1024;

        public ClientApp(IPAddress ip, int port = 11000) {
            this.serverIp = ip;
            this.port = port;
        }

        public bool Connect() {
            IPEndPoint serverEndPoint = new(serverIp, port);
            socket = new(serverEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(serverEndPoint);
            return true;
        }

        public Problem GetProblem(int id) {
            Connect();
            Request request = new("GET_PROBLEM", id, typeof(int));
            SendRequest(request);
            Response response = ReceiveResponse();
            return (Problem)response.Body!;
        }

        public List<(int, string)> GetProblemsIdsNames() {
            Connect();
            Request request = new("GET_PROBLEMS_IDS_NAMES", "empty", typeof(string));
            SendRequest(request);
            Response response = ReceiveResponse();
            return (List<(int, string)>)response.Body!;
        }

        public string SubmitSolution(Solution solution) {
            Connect();
            Request request = new("SUBMIT_SOLUTION", solution, typeof(Solution));
            SendRequest(request);
            Response response = ReceiveResponse();
            return (string)response.Body!;
        }
        public void SendRequest(Request request) {
            if (socket is null) {
                throw new InvalidOperationException("Client is not connected to the server.");
            }
            // Serialize the object to JSON and convert to bytes
            Console.WriteLine(request.ToJsonString());
            byte[] data = Encoding.UTF8.GetBytes(request.ToJsonString());
            int totalBytesSent = 0;
            int dataLength = data.Length;

            // Send data in chunks
            while (totalBytesSent < dataLength) {
                int bytesToSend = Math.Min(CHUNK_SIZE, dataLength - totalBytesSent);
                int bytesSent = socket.Send(data, totalBytesSent, bytesToSend, SocketFlags.None);
                totalBytesSent += bytesSent;
            }

            // Send the end token to signify the end of the data
            byte[] endTokenBytes = Encoding.UTF8.GetBytes(END_TOKEN);
            socket.Send(endTokenBytes);
        }
        public Response ReceiveResponse() {
            if (socket is null) {
                throw new InvalidOperationException("Client is not connected to the server.");
            }
            byte[] buffer = new byte[CHUNK_SIZE];
            StringBuilder stringBuilder = new();
            while (true) {
                int bytesRecieved = socket.Receive(buffer);
                string lastRecieved = Encoding.UTF8.GetString(buffer, 0, bytesRecieved);
                if (lastRecieved.EndsWith(END_TOKEN)) {
                    stringBuilder.Append(lastRecieved[..^END_TOKEN.Length]);
                    break;
                }
                stringBuilder.Append(lastRecieved);
            }
            string jsonString = stringBuilder.ToString();
            Response response = new(jsonString);
            return response;
        }



        public void Disconnect() {
            if (socket is null) {
                throw new InvalidOperationException("Client is not connected to the server.");
            }
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
