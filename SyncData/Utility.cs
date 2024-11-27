using System;
using System.IO;

namespace SyncData
{
    public static class Utility
    {
        public static void LogMessage(string status, string message, bool verbose, bool logToFile)
        {
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}:{status}:{message}";
            if (logToFile)
            {
                try
                {
                    File.AppendAllText("syncData.log", logMessage + Environment.NewLine);
                }
                catch (IOException ex)
                {
                    if (verbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Failed to write to log file: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}