using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace AutomatedJMeterTester
{
    static class Program
    {
        private static int _delay;
        
        private static void Main()
        {
            var stoppingHour = int.Parse(ConfigurationManager.AppSettings["stoppingHour"]);
            _delay = int.Parse(ConfigurationManager.AppSettings["loopDelayInMinutes"]);
            var delayInmilliseconds = _delay * 60000;

            var dateTime = DateTime.Now;

            var numberOfTestsDone = 0;

            while (dateTime.Hour < stoppingHour)
            {
                LaunchJMeterScript();
                numberOfTestsDone++;
                Thread.Sleep(delayInmilliseconds);
            }

            Console.WriteLine(
                $"AutoTester is done for the day! Number of tests done: {numberOfTestsDone}! Press any key to close...");

            Console.ReadKey();
        }

        private static void LaunchJMeterScript()
        {
            var launchTime = DateTime.Now;
            
            WriteConsoleMessage($"Launching JMeter Test at {launchTime}...");

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

           launchTime = launchTime.AddMinutes(_delay);

            WriteConsoleMessage($"JMeter script done! Next launch at {launchTime}");
        }

        private static void WriteConsoleMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}