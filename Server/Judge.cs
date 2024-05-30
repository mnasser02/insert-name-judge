using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Modules;

namespace Server
{
    public delegate Task<double> RunAsync(Solution solution, Testcase testcase, int timeLimit = 1);

    public static partial class Judge
    {
        public static async Task<string> CheckSolution(Solution solution, List<Testcase> testcases)
        {
            string path = "";
            try
            {
                (Process, string) process_path = solution.Lang switch
                {
                    "java" => await CreateJavaProcessAsync(solution),
                    "py" => await CreatePythonProcessAsync(solution),
                    "cpp" => await CreateCppProcessAsync(solution),
                    _ => throw new Exception("Unsupported language"),
                };
                var process = process_path.Item1;
                path = process_path.Item2;
                for (int i = 0; i < testcases.Count; i++)
                {
                    var testcase = testcases[i];
                    double executionTime = await RunProcess(process, testcase);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            return "Accepted";
        }

        public static async Task<double> RunProcess(
            Process process,
            Testcase testcase,
            int timeLimit = 1
        )
        {
            Stopwatch stopwatch = new();
            if (
                !await Task.Run(() =>
                {
                    stopwatch = Stopwatch.StartNew();
                    process.Start();
                    process.StandardInput.WriteLine(testcase.Input);
                    bool result = process.WaitForExit(timeLimit * 1000);
                    stopwatch.Stop();
                    return result;
                })
            )
            {
                process.Kill();
                throw new Exception("Time limit exceeded");
            }
            string output = await process.StandardOutput.ReadToEndAsync();
            if (output.Trim() != testcase.Output.Trim())
            {
                throw new Exception(
                    $"Wrong answer testcase {testcase.Id}, Expected: {testcase.Output}, Got: {output}"
                );
            }
            return stopwatch.Elapsed.TotalSeconds;
        }

        public static async Task<(Process, string)> CreateJavaProcessAsync(Solution solution)
        {
            string now = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), now); // Using temp directory for isolation
            Directory.CreateDirectory(dirPath); // Create the directory
            Regex classNameRegex = new(@"\bclass\s+(\w+)");
            Match match = classNameRegex.Match(solution.Code);
            if (!match.Success)
            {
                throw new Exception("Compilation Error");
            }
            string className = match.Groups[1].Value;
            string javaFile = Path.Combine(dirPath, $"{className}.java");
            await File.WriteAllTextAsync(javaFile, solution.Code);
            ProcessStartInfo processStartInfo =
                new()
                {
                    FileName = "javac",
                    Arguments = $"\"{javaFile}\"",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            Process compileProcess = new() { StartInfo = processStartInfo };
            compileProcess.Start();
            await compileProcess.WaitForExitAsync();
            if (compileProcess.ExitCode != 0)
            {
                throw new Exception("Compilation Error");
            }
            Console.WriteLine(javaFile[..^5]);
            processStartInfo = new()
            {
                FileName = "java",
                Arguments = $"{className}",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = dirPath
            };
            Process runProcess = new() { StartInfo = processStartInfo };
            return (runProcess, dirPath);
        }

        public static async Task<(Process, string)> CreatePythonProcessAsync(Solution solution)
        {
            string now = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string pyFile = now + ".py";
            await File.WriteAllTextAsync(pyFile, solution.Code);
            ProcessStartInfo processStartInfo =
                new()
                {
                    FileName = "python",
                    Arguments = $"{pyFile}",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            Process runProcess = new() { StartInfo = processStartInfo };
            return (runProcess, pyFile);
        }

        public static async Task<(Process, string)> CreateCppProcessAsync(Solution solution)
        {
            string now = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string cppFile = now + ".cpp";
            string exeFile = now + ".exe";
            await File.WriteAllTextAsync(cppFile, solution.Code);
            ProcessStartInfo processStartInfo =
                new()
                {
                    FileName = "g++",
                    Arguments = $"{cppFile} -o {exeFile}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            Process compileProcess = new() { StartInfo = processStartInfo };
            compileProcess.Start();
            await compileProcess.WaitForExitAsync();
            if (compileProcess.ExitCode != 0)
            {
                throw new Exception("Compilation Error");
            }
            if (File.Exists(cppFile))
            {
                File.Delete(cppFile);
            }
            processStartInfo = new()
            {
                FileName = exeFile,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process runProcess = new() { StartInfo = processStartInfo };
            return (runProcess, exeFile);
        }
    }
}