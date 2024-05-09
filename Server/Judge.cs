using Modules;
namespace Server {
    public static class Judge {
        public static string CheckSolution(Solution solution, Problem problem) {
            for (int i = 0; i < problem.Testcases.Length; i++) {
                var testcase = problem.Testcases[i];
                string verdict = ExecuteSolution(solution, testcase);
                if (verdict != "Accepted") {
                    return $"{verdict} on test {i}";
                }

            }
            return "Accepted";
        }
        public static string ExecuteSolution(Solution solution, Testcase testcase) {
            string output = "";
            double executionTime = 0;
            double memoryUsage = 0;

            // Execute the solution on the testcase


            if (executionTime > 1) {
                return "Time limit exceeded";
            }
            else if (memoryUsage > 256) {
                return "Memory limit exceeded";
            }
            else if (output != testcase.ExpectedOutput) {
                return "Wrong answer";
            }
            else {
                return "Accepted";
            }

        }

    }

}
