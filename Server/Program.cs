using System.Net;
using Microsoft.Extensions.Configuration;
using Modules;


namespace Server {
    internal class Program {

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables()
           .Build();
        static void Main(string[] args) {
            String connectionString = Configuration.GetConnectionString("PostgresConnection")!;
            Console.WriteLine(connectionString);
            IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
            IPAddress ip = iPHostEntry.AddressList[0];

            Server server = new(connectionString, ip);

            const int PORT = 11000;
            server.StartServer(PORT);

            Problem problem = server.GetProblem(1);
            Console.WriteLine(problem.Name);
            Console.WriteLine(problem.Statement);
            Console.WriteLine(problem.InputFormat);
            Console.WriteLine(problem.OutputFormat);
            Console.WriteLine(problem.Rating);
            Console.WriteLine();

            List<(int, string)> problems = server.GetProblemsIdsNames();
            foreach (var (id, name) in problems) {
                Console.WriteLine($"{id} {name}");
            }
            Console.WriteLine();

        }
    }
}
