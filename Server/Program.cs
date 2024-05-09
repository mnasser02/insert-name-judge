﻿using System.Net;
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
        }
    }
}