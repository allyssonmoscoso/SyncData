using System;
using System.IO;

namespace SyncData.Logging
{
    /// <summary>
    /// Logs messages to a file
    /// </summary>
    public class FileLogger : Logger
    {
        private readonly string _logFilePath;

        public FileLogger(string logFilePath = "syncData.log")
        {
            _logFilePath = logFilePath;
        }

        public override void Log(string status, string message)
        {
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}:{status}:{message}";
            try
            {
                File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
            }
            catch (IOException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed to write to log file: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
