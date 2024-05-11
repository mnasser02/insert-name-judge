namespace Modules
{
    public class Testcase
    {
        // Properties
        public int Id { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public int ProblemId { get; set; }

        // Constructor
        public Testcase(int id, string input, string output, int problemId)
        {
            Id = id;
            Input = input;
            Output = output;
            ProblemId = problemId;
        }
    }
}
