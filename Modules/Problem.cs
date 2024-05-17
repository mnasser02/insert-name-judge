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

        public Problem(
            int index,
            string name,
            string statement,
            int rating,
            string input,
            string output
        )
        {
            Id = index;
            Name = name;
            Statement = statement;
            Rating = rating;
            InputFormat = input;
            OutputFormat = output;
        }
    }
}
