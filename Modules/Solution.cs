using System;
using System.Text.Json;
using Newtonsoft.Json;

namespace Modules
{
    public class Solution
    {

        public int ProblemId { get; }
        public string Lang { get; }
        public string Code { get; }

        [JsonConstructor]
        public Solution(int problemId, string lang, string code)
        {
            ProblemId = problemId;
            Lang = lang;
            Code = code;
        }

        public Solution(int problemId, string solutionPath)
        {
            ProblemId = problemId;
            string[] solutionPathStrip = solutionPath.Split(".");
            Lang = solutionPathStrip.Last();
            Code = File.ReadAllText(solutionPath);
        }
    }
}
