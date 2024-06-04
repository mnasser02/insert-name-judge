using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules
{
    public class Problem
    {
        public int Id { get; }
        public string Name { get; }
        public string Statement { get; }
        public int Rating { get; }
        public string InputFormat { get; }
        public string OutputFormat { get; }

        public string ExampleOutput { get; }
        public string ExampleInput { get; }

        public Problem(
            int id,
            string name,
            string statement,
            int rating,
            string inputformat,
            string outputformat,
            string exampleInput,
            string exampleOutput

        )
        {
            Id = id;
            Name = name;
            Statement = statement;
            Rating = rating;
            InputFormat = inputformat;
            OutputFormat = outputformat;
            ExampleInput = exampleInput;
            ExampleOutput = exampleOutput;
        }
    }
}
