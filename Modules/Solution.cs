using System;
using System.Text.Json;

namespace Modules
{
    public class Solution
    {
        private readonly int problemID;
        private readonly string lang;
        private readonly string code;

        public Solution(int problemID, string lang, string code)
        {
            this.problemID = problemID;
            this.lang = lang;
            this.code = code;
        }

        public Solution(int problemID, string solutionPath)
        {
            this.problemID = problemID;
            string[] solutionPathStrip = solutionPath.Split(".");
            lang = solutionPathStrip.Last();
            code = File.ReadAllText(solutionPath);
        }

        public Solution(JsonElement json)
        {
            lang = json.GetProperty("lang").GetString()!;
            code = json.GetProperty("code").GetString()!;
        }

        public string Lang
        {
            get => lang;
        }

        public string Code
        {
            get => code;
        }
    }
}
