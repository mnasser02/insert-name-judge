using Npgsql;
using System.Net.Sockets;
using System.Net;
using Modules;
namespace Server {
    public class Server {
        private readonly string _connectionString;
        private readonly IPAddress _ip;

        public Server(string connectionString, IPAddress ip) { 
            _connectionString = connectionString;
            _ip = ip;
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

        //public string SubmitSolution(Solution solution, int problemId) {
        //    Problem problem = GetProblem(problemId);
        //    string verdict = Judge.CheckSolution(solution, problem);
        //    return verdict;
        //}

        public void StartServer(int port) {
            TcpListener server = new(_ip, port);
            server.Start();

            Console.WriteLine($"Server started on port {port}.");

            //while (true) {
            //    TcpClient client = server.AcceptTcpClient();

            //    Task.Run(() => HandleClient(client));
            //}
        }
        /*
        private void HandleClient(TcpClient client) {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new(stream);
            StreamWriter writer = new(stream) { AutoFlush = true };

            try {
                string request = reader.ReadLine(); // Read request
                string[] requestParts = request.Split(':');
                string action = requestParts[0];
                if(action == "GET_PROBLEMS_IDS_NAMES") {
                    List<(int, string)> values = GetProblemsIdsNames();
                    string response = string.Join(":", values.Select(v => $"{v.Item1},{v.Item2}"));
                    writer.WriteLine(response);
                }
                else if (action == "GET_PROBLEM") {
                    int problemId = int.Parse(requestParts[1]); 
                    Problem problem = GetProblem(problemId);
                    string response = $"{problem.Id}:{problem.Statement}:{problem.InputFormat}:{problem.OutputFormat}:{problem.Notes}";
                    writer.WriteLine(response); 
                }
                else if (action == "SUBMIT_SOLUTION") {
                    int problemId = int.Parse(requestParts[1]);
                    string lang = requestParts[2];
                    string code = requestParts[3]; 
                    Solution solution = new Solution(code);
                    string response = $"{solution.Id}:{SubmitSolution(solution, problemId)}";
                    writer.WriteLine(response); 
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally {
                client.Close();
            }
        }*/
    }
}
