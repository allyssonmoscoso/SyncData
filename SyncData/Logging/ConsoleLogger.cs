using System;

namespace SyncData.Logging
{
    /// <summary>
    /// Logs messages to the console with color coding
    /// </summary>
    public class ConsoleLogger : Logger
    {
        private readonly bool _verbose;

        public ConsoleLogger(bool verbose)
        {
            _verbose = verbose;
        }

        public override void Log(string status, string message)
        {
            if (!_verbose && status != "Error")
            {
                return;
            }

            var color = status switch
            {
                "Error" => ConsoleColor.Red,
                "Success" => ConsoleColor.Green,
                "Info" => ConsoleColor.Cyan,
                _ => ConsoleColor.White
            };

            Console.ForegroundColor = color;
            Console.WriteLine($"{status}: {message}");
            Console.ResetColor();
        }
    }
}
