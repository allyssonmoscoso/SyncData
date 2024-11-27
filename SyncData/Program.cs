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

            LogMessage("Error", "You must provide two directory paths as arguments.", verbose, logToFile);
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

            LogMessage("Error", "The second path cannot be the same as the first.", verbose, logToFile);
            return;
        }

        if (Directory.Exists(path1) && Directory.Exists(path2))
        {
            SynchronizeDirectories(path1, path2, verbose, logToFile);
            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Synchronization completed.");
                Console.ResetColor();
            }

            LogMessage("Success", "Synchronization completed.", verbose, logToFile);
        }
        else
        {
            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("One or both paths do not exist.");
                Console.ResetColor();
            }

            LogMessage("Error", "One or both paths do not exist.", verbose, logToFile);
        }

        static void LogMessage(string status, string message, bool verbose, bool logToFile)
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

        static void SynchronizeDirectories(string sourceDir, string targetDir, bool verbose, bool logToFile)
        {
            var sourceDirectory = new DirectoryInfo(sourceDir);
            var targetDirectory = new DirectoryInfo(targetDir);

            var filesToCopy = sourceDirectory.GetFiles().Length + targetDirectory.GetFiles().Length;
            var directoriesToCreate = sourceDirectory.GetDirectories().Length + targetDirectory.GetDirectories().Length;
            var totalOperations = filesToCopy + directoriesToCreate;

            using (var progressBar = new ProgressBar())
            {
                int progress = 0;

                // Synchronize files from source directory to target directory
                foreach (var file in sourceDirectory.GetFiles())
                {
                    var targetFilePath = Path.Combine(targetDir, file.Name);
                    if (!File.Exists(targetFilePath) || file.LastWriteTime > File.GetLastWriteTime(targetFilePath))
                    {
                        var startTime = DateTime.Now;
                        file.CopyTo(targetFilePath, true);
                        var endTime = DateTime.Now;
                        if (verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"File synchronized: {file.FullName} -> {targetFilePath} (Time: {(endTime - startTime).TotalMilliseconds} ms)");
                            Console.ResetColor();
                        }
                        LogMessage("Success", $"File synchronized: {file.FullName} -> {targetFilePath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                    }

                    progress++;
                    progressBar.Report((double)progress / totalOperations);
                }

                // Synchronize files from target directory to source directory
                foreach (var file in targetDirectory.GetFiles())
                {
                    var sourceFilePath = Path.Combine(sourceDir, file.Name);
                    if (!File.Exists(sourceFilePath) || file.LastWriteTime > File.GetLastWriteTime(sourceFilePath))
                    {
                        var startTime = DateTime.Now;
                        file.CopyTo(sourceFilePath, true);
                        var endTime = DateTime.Now;
                        if (verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"File synchronized: {file.FullName} -> {sourceFilePath} (Time: {(endTime - startTime).TotalMilliseconds} ms)");
                            Console.ResetColor();
                        }
                        LogMessage("Success", $"File synchronized: {file.FullName} -> {sourceFilePath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                    }

                    progress++;
                    progressBar.Report((double)progress / totalOperations);
                }

                // Synchronize subdirectories
                foreach (var directory in sourceDirectory.GetDirectories())
                {
                    var targetSubDirPath = Path.Combine(targetDir, directory.Name);
                    if (!Directory.Exists(targetSubDirPath))
                    {
                        var startTime = DateTime.Now;
                        Directory.CreateDirectory(targetSubDirPath);
                        var endTime = DateTime.Now;
                        if (verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"Directory created: {targetSubDirPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)");
                            Console.ResetColor();
                        }
                        LogMessage("Success", $"Directory created: {targetSubDirPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                    }

                    SynchronizeDirectories(directory.FullName, targetSubDirPath, verbose, logToFile);
                    progress++;
                    progressBar.Report((double)progress / totalOperations);
                }

                foreach (var directory in targetDirectory.GetDirectories())
                {
                    var sourceSubDirPath = Path.Combine(sourceDir, directory.Name);
                    if (!Directory.Exists(sourceSubDirPath))
                    {
                        var startTime = DateTime.Now;
                        Directory.CreateDirectory(sourceSubDirPath);
                        var endTime = DateTime.Now;
                        if (verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"Directory created: {sourceSubDirPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)");
                            Console.ResetColor();
                        }
                        LogMessage("Success", $"Directory created: {sourceSubDirPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                    }

                    SynchronizeDirectories(directory.FullName, sourceSubDirPath, verbose, logToFile);
                    progress++;
                    progressBar.Report(1.0);
                }
            }
        }
    }
}  
}
