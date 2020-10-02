using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace AutomatedJMeterTester
{
    internal static class Program
    {
        private static void Main()
        {
            var stoppingHour = int.Parse(ConfigurationManager.AppSettings["stoppingHour"]);
            var delay = int.Parse(ConfigurationManager.AppSettings["loopDelayInMinutes"]) * 60000;

            var dateTime = DateTime.Now;

            var numberOfTestsDone = 0;

            while (dateTime.Hour < stoppingHour)
            {
                LaunchJMeterScript();
                numberOfTestsDone++;
                Thread.Sleep(delay);
            }

            Console.WriteLine(
                $"AutoTester is done for the day! Number of tests done: {numberOfTestsDone}! Press any key to close...");

            Console.ReadKey();
        }

        private static void LaunchJMeterScript()
        {
            Console.WriteLine($"Launching JMeter Test at {DateTime.Now}...");

            var processInfo =
                new ProcessStartInfo("cmd.exe", "/c " + ConfigurationManager.AppSettings["batchFileLocation"])
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

            var process = Process.Start(processInfo);

            if (process == null) return;
            
            process.OutputDataReceived += (sender, e) =>
                Console.WriteLine(e.Data);
            process.BeginOutputReadLine();

            process.WaitForExit();
            process.Close();
        }
    }
}