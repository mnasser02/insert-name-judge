using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Npgsql;
using Modules;

namespace Server {
    public class Server {
        private readonly string _connectionString;
        private readonly IPAddress _ip;

        private static readonly string END_TOKEN = "!##<|EOF|>";
        private const int CHUNK_SIZE = 1024;

        public Server(string connectionString, IPAddress ip) {
            _connectionString = connectionString;
            _ip = ip;
        }

        public void StartServer(int port) {
            // create a socket that uses TCP
            Console.WriteLine("ip address: " + _ip.ToString());
            Socket socket = new(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipEndPoint = new(_ip, port);

            // associate the socket with a local endpoint
            socket.Bind(ipEndPoint);

            // specify how many requests a Socket can listen before it gives Server busy response
            socket.Listen(100);

            Console.WriteLine($"Server listening on {ipEndPoint}");

            while (true) {
                // accept incoming connection requests
                Socket handler = socket.Accept();
                Console.WriteLine($"Server connected to client {handler.RemoteEndPoint}");

                Thread clientThread = new(new ParameterizedThreadStart(HandleClient));
                Console.WriteLine("working on thread: " + clientThread.ManagedThreadId);
                clientThread.Start(handler);
            }

        }
        public void HandleClient(object? obj) {
            if (obj == null) {
                throw new InvalidOperationException("Client is not connected to the server.");
            }

            Socket handler = (Socket)obj;

            // receive request
            Request request = ReceiveRequest(handler);
            Console.WriteLine($"Request Type: {request.Type}, Body: {request.Body}, Body type: {request.Body!.GetType()}");

            Response response;
            // check type of request
            switch (request.Type) {
                case "GET_PROBLEM":
                    Problem problem = GetProblem((int)request.Body!);
                    response = new Response(problem, typeof(Problem));
                    break;
                case "GET_PROBLEMS_IDS_NAMES":
                    List<(int, string)> problems = GetProblemsIdsNames();
                    response = new Response(problems, typeof(List<(int, string)>));
                    break;
                case "SUBMIT_SOLUTION":
                    Solution solution = (Solution)request.Body!;
                    string verdict = SubmitSolution(solution).Result;
                    response = new Response(verdict, typeof(string));
                    break;
                default:
                    throw new InvalidOperationException("Invalid request type.");

            }
            //Task.Delay(10000).Wait(); // to test multi-threading
            SendResponse(handler, response);
            Console.WriteLine();

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        public List<(int, string)> GetProblemsIdsNames() {
            using (var conn = new NpgsqlConnection(_connectionString)) {
                conn.Open();

                string query = "SELECT id, name FROM problem";
                using (var cmd = new NpgsqlCommand(query, conn)) {
                    using (var reader = cmd.ExecuteReader()) {
                        List<(int, string)> problems = new();
                        while (reader.Read()) {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            problems.Add((id, name));
                        }
                        return problems;
                    }
                }
            }
        }
        public Problem GetProblem(int problemId) {
            using (var conn = new NpgsqlConnection(_connectionString)) {
                conn.Open();

                string query = "SELECT * FROM problem WHERE id = @id";
                using (var cmd = new NpgsqlCommand(query, conn)) {
                    cmd.Parameters.AddWithValue("id", problemId);
                    using (var reader = cmd.ExecuteReader()) {
                        reader.Read();
                        // problem table: id , name, statement, input_format, output_format, notes, rating
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        string statement = reader.GetString(2);
                        string input = reader.GetString(3);
                        string output = reader.GetString(4);
                        int rating = reader.GetInt32(6);
                        return new Problem(id, name, statement, rating, input, output);
                    }
                }
            }
        }

        public async Task<string> SubmitSolution(Solution solution) {
            Console.WriteLine(solution.Code);
            Problem problem = GetProblem(solution.ProblemId);
            List<Testcase> testcases = GetTestcases(problem.Id);
            string verdict = await Judge.CheckSolution(solution, testcases);
            return verdict;
        }

        public List<Testcase> GetTestcases(int problemId) {
            using (var conn = new NpgsqlConnection(_connectionString)) {
                conn.Open();

                string query = "SELECT * FROM testcase WHERE problem_id = @problem_id";
                using (var cmd = new NpgsqlCommand(query, conn)) {
                    cmd.Parameters.AddWithValue("problem_id", problemId);
                    using (var reader = cmd.ExecuteReader()) {
                        List<Testcase> testcases = new();
                        while (reader.Read()) {
                            int id = reader.GetInt32(0);
                            string input = reader.GetString(1);
                            string output = reader.GetString(2);
                            testcases.Add(new Testcase(id, input, output, problemId));
                        }
                        return testcases;
                    }
                }
            }
        }
        public Request ReceiveRequest(Socket socket) {
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
            Console.WriteLine($"Request: {jsonString}");
            return new Request(jsonString);
        }
        public void SendResponse(Socket socket, Response response) {
            Console.WriteLine($"Response: {response.ToJsonString()}");
            Console.WriteLine();
            // Serialize the object to JSON and convert to bytes
            byte[] data = Encoding.UTF8.GetBytes(response.ToJsonString());
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


    }
}
