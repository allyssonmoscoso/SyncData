using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SyncData
{
    public static class Utility
    {
        private static async Task<bool> UploadFileToFtp(string fileName, string filePath, string ftpServerUrl, string username, string password)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(ftpServerUrl);
                    var byteArray = new NetworkCredential(username, password).GetCredential(client.BaseAddress, "Basic").Password;
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes($"{username}:{byteArray}")));

                    byte[] fileContents = await File.ReadAllBytesAsync(filePath);
                    var content = new ByteArrayContent(fileContents);
                    var response = await client.PutAsync(fileName, content);

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error =>", ex);
            }
        }

        public static async Task SynchronizeDirectories(string sourceDir, string targetDir, bool verbose, bool logToFile, bool exclude, List<string> excludePaths, bool useFtp, string userFtp, string passwordFtp)
        {
            var sourceDirectory = new DirectoryInfo(sourceDir);
            var targetDirectory = new DirectoryInfo(targetDir);

            var filesToCopy = sourceDirectory.GetFiles().Length + targetDirectory.GetFiles().Length;
            var directoriesToCreate = sourceDirectory.GetDirectories().Length + targetDirectory.GetDirectories().Length;
            var totalOperations = filesToCopy + directoriesToCreate;

            using var progressBar = new ProgressBar();
            {
                int progress = 0;

                // Synchronize files from source directory to target directory
                foreach (var file in sourceDirectory.GetFiles())
                {
                    if (excludePaths.Any(e => file.FullName.Contains(e, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Excluding file: " + file.FullName);
                            Console.ResetColor();
                        }

                        if (logToFile)
                        {
                            LogMessage("Success", $"Excluding file: {file.FullName}", verbose, logToFile);
                        }

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

                        if (logToFile)
                        {
                            LogMessage("Success", $"File synchronized: {file.FullName} -> {targetFilePath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                        }

                        if (useFtp)
                        {
                            await UploadFileToFtp(file.Name, targetFilePath, "ftpServerUrl", userFtp, passwordFtp);
                        }
                    }

                    progress++;
                    progressBar.Report((double)progress / totalOperations);
                }

                // Synchronize files from target directory to source directory
                foreach (var file in targetDirectory.GetFiles())
                {
                    if (excludePaths.Any(e => file.FullName.Contains(e, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Excluding file: " + file.FullName);
                            Console.ResetColor();
                        }

                        if (logToFile)
                        {
                            LogMessage("Success", $"Excluding file: {file.FullName}", verbose, logToFile);
                        }
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

                        if (logToFile)
                        {
                            LogMessage("Success", $"File synchronized: {file.FullName} -> {sourceFilePath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                        }

                        if (useFtp)
                        {
                            await UploadFileToFtp(file.Name, sourceFilePath, "ftpServerUrl", userFtp, passwordFtp);
                        }
                    }

                    progress++;
                    progressBar.Report((double)progress / totalOperations);
                }

                // Synchronize subdirectories
                foreach (var directory in sourceDirectory.GetDirectories())
                {
                    if (excludePaths.Any(e => directory.FullName.Contains(e, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Excluding directory: " + directory.FullName);
                            Console.ResetColor();
                        }

                        if (logToFile)
                        {
                            LogMessage("Success", $"Excluding directory: {directory.FullName}", verbose, logToFile);
                        }
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

                        if (logToFile)
                        {
                            LogMessage("Success", $"Directory created: {targetSubDirPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                        }
                    }

                    await SynchronizeDirectories(directory.FullName, targetSubDirPath, verbose, logToFile, exclude, excludePaths, useFtp, userFtp, passwordFtp);
                    progress++;
                    progressBar.Report((double)progress / totalOperations);
                }

                foreach (var directory in targetDirectory.GetDirectories())
                {
                    if (excludePaths.Any(e => directory.FullName.Contains(e, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (verbose)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine("Excluding directory: " + directory.FullName);
                            Console.ResetColor();
                        }

                        if (logToFile)
                        {
                            LogMessage("Success", $"Excluding directory: {directory.FullName}", verbose, logToFile);
                        }
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

                        if (logToFile)
                        {
                            LogMessage("Success", $"Directory created: {sourceSubDirPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                        }
                    }

                    await SynchronizeDirectories(directory.FullName, sourceSubDirPath, verbose, logToFile, exclude, excludePaths, useFtp, userFtp, passwordFtp);
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
