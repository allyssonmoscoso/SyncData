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
            bool ftp = false;
            string source = string.Empty;
            string target = string.Empty;
            List<string> excludePaths = new List<string>();

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
                    case  "-exclude=":
                        exclude = true;
                        excludePaths.AddRange(arg.Substring(9).Trim('{', '}').Split(',').Where(p => !string.IsNullOrWhiteSpace(p)));
                        break;
                    case "-ftp":
                        ftp = true;
                        break;   
                    default:
                        if (arg.StartsWith("-source="))
                        {
                            source = arg.Substring(8);
                        }
                        else if (arg.StartsWith("-target="))
                        {
                            target = arg.Substring(8);
                        }
                        break;
                }
            }

            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
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
            

            if (string.Equals(source, target, StringComparison.OrdinalIgnoreCase))
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

                if (Directory.Exists(source) && Directory.Exists(target))
                {
                    Utility.SynchronizeDirectories(source, target, verbose, logToFile, exclude, excludePaths, ftp);
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
