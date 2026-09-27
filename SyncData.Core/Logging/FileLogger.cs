using System;
using System.IO;
using SyncData.Configuration;
using SyncData.Core.Localization;

namespace SyncData.Logging
{
    /// <summary>
    /// Logs messages to a file
    /// </summary>
    public class FileLogger : Logger
    {
        private readonly string _logFilePath;

        public FileLogger(string? logFilePath = null)
        {
            _logFilePath = logFilePath ?? AppConstants.DefaultLogFileName;
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
                Console.WriteLine(CoreLocalizer.Format("Log_FileWriteFailed", ex.Message));
                Console.ResetColor();
            }
        }
    }
}
