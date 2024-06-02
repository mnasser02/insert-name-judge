using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Modules;

namespace Client {
    internal class Program {
        static void Main(string[] args) {

            IPHostEntry ipHostEntry = Dns.GetHostEntry("localhost");
            IPAddress serverIp = ipHostEntry.AddressList[0];

            ClientApp client = new(serverIp);

            Problem problem = client.GetProblem(1);
            Console.WriteLine("Problem id: " + problem.Id);
            Console.WriteLine("Problem Name: " + problem.Name);
            Console.WriteLine("Problem Statement: " + problem.Statement);
            Console.WriteLine("Problem Input Format: " + problem.InputFormat);
            Console.WriteLine("Problem Output Format: " + problem.OutputFormat);
            Console.WriteLine("Problem Rating: " + problem.Rating);
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------");

            List<(int, string)> problems = client.GetProblemsIdsNames();
            foreach (var (id, name) in problems) {
                Console.WriteLine($"{id} {name}");
            }
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------");

            Console.WriteLine(problems[0].ToString());
            Console.WriteLine();
            // sum of 2 ints
            Solution solution = new(1, "cpp", "#include <iostream>\nint main() {\nint a, b;\nstd::cin >> a >> b;\nstd::cout << a + b;\nreturn 0;\n}\n");
            string verdict = client.SubmitSolution(solution);
            Console.WriteLine(verdict);
            Console.WriteLine();
            Console.WriteLine("---------------------------------------------------");
        }
    }
}
