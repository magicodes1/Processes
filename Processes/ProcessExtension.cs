using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Processes
{
    public interface IProcessExtension
    {
        public string FileName { get; }

        public string[] Arguments { get; }

        public string WorkingDirectory { get; }

        ProcessExtension Wrap(string command);

        ProcessExtension WithArgument(params string[] arguments);

        ProcessExtension WithWorkingDirectory(string workingDirectory);
    }


    public class ProcessExtension : IProcessExtension
    {

        public string FileName { get; }

        public string[] Arguments { get; }

        public string WorkingDirectory { get; }

        public string StandardOuput { get; private set; } = string.Empty;

        public string StandardError { get; private set; } = string.Empty;

        public string StandardInput { get; private set; } = string.Empty;


        public ProcessExtension()
        {
        }

        public ProcessExtension(
            string fileName,
            string[] argument,
            string workingDirectory)
        {
            FileName = fileName;
            Arguments = argument;
            WorkingDirectory = workingDirectory;
        }


        public ProcessExtension Wrap(string command)
        {
            return new ProcessExtension(command, Arguments, WorkingDirectory);
        }

        public ProcessExtension WithArgument(params string[] arguments)
        {
            return new ProcessExtension(FileName, arguments, WorkingDirectory);
        }

        public ProcessExtension WithWorkingDirectory(string workingDirectory)
        {
            return new ProcessExtension(FileName, Arguments, workingDirectory);
        }

        public async Task<ResultProcess> ExcuteAsync()
        {
            var info = CreateProcessInfo();

            Process process = new() { StartInfo = info };

            process.Start();

            string running = IsProcessRunning(process.Id) ? "running" : "not running";

            Console.WriteLine($"Process {process.ProcessName} is {running}");

            StandardOuput = process.StandardOutput.ReadToEnd();

            StandardError = process.StandardError.ReadToEnd();

            process.StandardInput.WriteLine(StandardInput);

            await process.WaitForExitAsync();

            Console.WriteLine("Output " + StandardOuput);
            Console.WriteLine("Error " + StandardError);
            Console.WriteLine("input " + StandardInput);

            string exit = process.HasExited ? "exited" : "running";

            TimeSpan exitedTime = process.ExitTime - process.StartTime;

            Console.WriteLine($"{process.ProcessName} is {exit} in {exitedTime}");

            return new ResultProcess
            {
                Id = process.Id,
                Name = process.ProcessName,
                Output = StandardOuput,
                Error = StandardError,
                StartTime = process.StartTime,
                EndTime = process.ExitTime,
                ExitTime = exitedTime,
            };
        }

        public void KillProcess(int id, int timeOut = 30000)
        {
            Process process = Process.GetProcessById(id);

            process.Kill();

            process.WaitForExit(timeOut);

            if (process.HasExited)
            {
                Console.WriteLine($"Process {process.ProcessName} is killed.");
            }
            else
            {
                Console.WriteLine($"Waited for {timeOut / 1000} seconds, but couldn't kill process {process.ProcessName}");
            }

        }

        private static bool IsProcessRunning(int id)
        {
            try
            {
                Process.GetProcessById(id);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }

        private ProcessStartInfo CreateProcessInfo()
        {
            string argument = string.Join(" ", Arguments);

            ProcessStartInfo processStartInfo = new()
            {
                FileName = FileName,
                Arguments = argument,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = WorkingDirectory,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
            };

            return processStartInfo;
        }

    }
}
