<<<<<<< HEAD
﻿using System.Threading.Tasks;
using SyncData.Configuration;
using SyncData.Core;
using SyncData.Logging;
=======
﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
>>>>>>> 22f4bc6a82bbabbd9eb97fca4fab5fa39c359557

namespace SyncData
{
    class Program
    {
        public static async Task Main(string[] args)
        {
<<<<<<< HEAD
            // Parse command-line arguments
            var parser = new ArgumentParser();
            var config = parser.Parse(args);

            // Create logger based on configuration
            var logger = CreateLogger(config);

            // Create and run the application
            var app = new SyncApplication(config, logger);
            await app.RunAsync();
        }

        private static Logger CreateLogger(SyncConfiguration config)
        {
            var compositeLogger = new CompositeLogger();
            
            // Always add console logger
            compositeLogger.AddLogger(new ConsoleLogger(config.Verbose));
            
            // Add file logger if requested
            if (config.LogToFile)
            {
                compositeLogger.AddLogger(new FileLogger());
=======
            var options = ParseArguments(args);

            if (!ValidatePaths(options.Source, options.Target, options.LogToFile, options.Verbose)) return;

            if (options.Exclude && !ValidateExcludePaths(options.ExcludePaths, options.LogToFile, options.Verbose)) return;

            if (Directory.Exists(options.Source) && Directory.Exists(options.Target))
            {
                Utility.SynchronizeDirectories(
                    options.Source, 
                    options.Target, 
                    options.Verbose, 
                    options.LogToFile, 
                    options.Exclude, 
                    options.ExcludePaths, 
                    options.Ftp, 
                    options.PreservePermissionsAndTimestamps, 
                    null, 
                    null
                );

                LogMessage("Synchronization completed.", "Success", options.LogToFile, options.Verbose, ConsoleColor.Green);
            }
            else
            {
                LogMessage("One or both paths do not exist.", "Error", options.LogToFile, options.Verbose, ConsoleColor.Red);
            }
        }

        private static (bool Verbose, bool LogToFile, bool Exclude, bool Ftp, bool PreservePermissionsAndTimestamps, string Source, string Target, List<string> ExcludePaths) ParseArguments(string[] args)
        {
            bool verbose = false, logToFile = false, exclude = false, ftp = false, preservePermissionsAndTimestamps = false;
            string source = string.Empty, target = string.Empty;
            var excludePaths = new List<string>();

            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "-v":
                    case "-verbose":
                        verbose = true;
                        break;
                    case "-log-file":
                        logToFile = true;
                        break;
                    case var excludeArg when excludeArg.StartsWith("-exclude="):
                        exclude = true;
                        excludePaths.AddRange(excludeArg.Substring(9).Trim('{', '}').Split(',').Select(p => p.Trim()).Where(p => !string.IsNullOrWhiteSpace(p)));
                        break;
                    case "-ftp":
                        ftp = true;
                        break;
                    case "-preserve":
                        preservePermissionsAndTimestamps = true;
                        break;
                    default:
                        if (arg.StartsWith("-source=")) source = arg.Substring(8);
                        else if (arg.StartsWith("-target=")) target = arg.Substring(8);
                        break;
                }
            }

            return (verbose, logToFile, exclude, ftp, preservePermissionsAndTimestamps, source, target, excludePaths);
        }

        private static bool ValidatePaths(string source, string target, bool logToFile, bool verbose)
        {
            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                LogMessage("You must provide two directory paths as arguments.", "Error", logToFile, verbose, ConsoleColor.Red);
                return false;
            }

            if (string.Equals(source, target, StringComparison.OrdinalIgnoreCase))
            {
                LogMessage("The second path cannot be the same as the first.", "Error", logToFile, verbose, ConsoleColor.Red);
                return false;
            }

            return true;
        }

        private static bool ValidateExcludePaths(List<string> excludePaths, bool logToFile, bool verbose)
        {
            if (excludePaths.Count == 0)
            {
                LogMessage("-exclude: You must provide at least one exclude path as an argument.", "Error", logToFile, verbose, ConsoleColor.Red);
                return false;
            }

            return true;
        }

        private static void LogMessage(string message, string logType, bool logToFile, bool verbose, ConsoleColor color)
        {
            if (verbose)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }

            if (logToFile)
            {
                Utility.LogMessage(logType, message, verbose, logToFile);
>>>>>>> 22f4bc6a82bbabbd9eb97fca4fab5fa39c359557
            }
            
            return compositeLogger;
        }
    }
}
