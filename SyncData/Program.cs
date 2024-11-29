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
            bool exclude = false;
            string path1 = string.Empty;
            string path2 = string.Empty;
            List<string> excludePaths = new List<string>();

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
                else if (arg.StartsWith("-exclude="))
                {   
                    exclude = true;
                    excludePaths.AddRange(arg.Substring(9).Trim('{', '}').Split(',').Where(p => !string.IsNullOrWhiteSpace(p)));
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
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You must provide two directory paths as arguments.");

                    if (logToFile)
                    {
                        Utility.LogMessage("Error", "You must provide two directory paths as arguments.", verbose,
                            logToFile);
                    }
                    return;
                }

                if (exclude)
                {
                    if (excludePaths.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("-exclude: You must provide at least one exclude path as an argument.");

                        if (logToFile)
                        {
                            Utility.LogMessage("Error", "-exclude: You must provide at least one exclude path as an argument.", verbose,
                                logToFile);    
                        }
                        
                        
                        return;
                    }
                }
            

            if (string.Equals(path1, path2, StringComparison.OrdinalIgnoreCase))
                {
                    if (verbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("The second path cannot be the same as the first.");
                        Console.ResetColor();
                    }

                    if (logToFile)
                    {
                        Utility.LogMessage("Error", "The second path cannot be the same as the first.", verbose, logToFile);
                            
                    }
                    return;
                    
                }

                if (Directory.Exists(path1) && Directory.Exists(path2))
                {
                    Utility.SynchronizeDirectories(path1, path2, verbose, logToFile, exclude, excludePaths);
                    if (verbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Synchronization completed.");
                        Console.ResetColor();
                    }

                    if (logToFile)
                    {
                        Utility.LogMessage("Success", "Synchronization completed.", verbose, logToFile);    
                    }
                    
                }
                else
                {
                    if (verbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("One or both paths do not exist.");
                        Console.ResetColor();
                    }

                    if (logToFile)
                    {
                        Utility.LogMessage("Error", "One or both paths do not exist.", verbose, logToFile);    
                    }
                    
                }
            }
        }
    }
