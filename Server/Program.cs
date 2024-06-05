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
            //Request req = (Request)Jsonifier.ToObject("{\"RequestType\":\"GET_PROBLEM\",\"Body\":1, \"ObjectType\":\"Modules.Request\"}");
            //Console.WriteLine(req.Body);
            const int PORT = 11000;
            //IPHostEntry iPHostEntry = Dns.GetHostEntry("localhost");
            //IPAddress ip = iPHostEntry.AddressList[0];

            IPAddress ip = IPAddress.Parse("192.168.137.212");

            Server server = new(connectionString, ip);

            server.StartServer(PORT);

        }
    }
}
