using System;
using System.IO;

namespace SyncData
{
    
    class Program
{
    public static void Main(string[] args)
    {
        bool verbose = false;
        bool logToFile = false;
        string path1 = string.Empty;
        string path2 = string.Empty;

        foreach (var arg in args)
        {
            if (arg == "-v" || arg == "-verbose")
            {
                verbose = true;
            }
            else if (arg == "-log-file")
            {
                logToFile = true;
            }
            else if (string.IsNullOrEmpty(path1))
            {
                path1 = arg;
            }
            else if (string.IsNullOrEmpty(path2))
            {
                path2 = arg;
            }
        }

        if (string.IsNullOrEmpty(path1) || string.IsNullOrEmpty(path2))
        {
            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You must provide two directory paths as arguments.");
                Console.ResetColor();
            }

            Utility.LogMessage("Error", "You must provide two directory paths as arguments.", verbose, logToFile);
            return;
        }

        if (string.Equals(path1, path2, StringComparison.OrdinalIgnoreCase))
        {
            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The second path cannot be the same as the first.");
                Console.ResetColor();
            }

            Utility.LogMessage("Error", "The second path cannot be the same as the first.", verbose, logToFile);
            return;
        }

        if (Directory.Exists(path1) && Directory.Exists(path2))
        {
            Utility.SynchronizeDirectories(path1, path2, verbose, logToFile);
            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Synchronization completed.");
                Console.ResetColor();
            }

            Utility.LogMessage("Success", "Synchronization completed.", verbose, logToFile);
        }
        else
        {
            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("One or both paths do not exist.");
                Console.ResetColor();
            }

            Utility.LogMessage("Error", "One or both paths do not exist.", verbose, logToFile);
        }     
    }
}  
}
