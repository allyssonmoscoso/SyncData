using System;
using System.IO;

namespace SyncData
{
    public static class Utility
    {

        public static void SynchronizeDirectories(string sourceDir, string targetDir, bool verbose, bool logToFile, List<string> exclude)
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
                    if (exclude.Any(e => file.FullName.StartsWith(e, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                
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
                        Utility.LogMessage("Success", $"File synchronized: {file.FullName} -> {targetFilePath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                    }

                    progress++;
                    progressBar.Report((double)progress / totalOperations);
                }
                
                // Synchronize files from target directory to source directory
                foreach (var file in targetDirectory.GetFiles())
                {
                    if (exclude.Any(e => file.FullName.StartsWith(e, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                
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
                        Utility.LogMessage("Success", $"File synchronized: {file.FullName} -> {sourceFilePath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                    }

                    progress++;
                    progressBar.Report((double)progress / totalOperations);
                }
                
                // Synchronize subdirectories
                foreach (var directory in sourceDirectory.GetDirectories())
                {
                    if (exclude.Any(e => directory.FullName.StartsWith(e, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                
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
                        Utility.LogMessage("Success", $"Directory created: {targetSubDirPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                    }

                    SynchronizeDirectories(directory.FullName, targetSubDirPath, verbose, logToFile, exclude);
                    progress++;
                    progressBar.Report((double)progress / totalOperations);
                }

                foreach (var directory in targetDirectory.GetDirectories())
                {
                    if (exclude.Any(e => directory.FullName.StartsWith(e, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                
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
                        Utility.LogMessage("Success", $"Directory created: {sourceSubDirPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                    }

                    SynchronizeDirectories(directory.FullName, sourceSubDirPath, verbose, logToFile, exclude);
                    progress++;
                    progressBar.Report(1.0);
                }
            }
        }

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