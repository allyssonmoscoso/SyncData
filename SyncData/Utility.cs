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
                    var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                    byte[] fileContents = await File.ReadAllBytesAsync(filePath);
                    using var content = new ByteArrayContent(fileContents);
                    var response = await client.PutAsync(fileName, content);

                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error =>", ex);
            }
        }

        public static async Task SynchronizeDirectories(string sourceDir, string targetDir, bool verbose, bool logToFile, bool exclude, List<string> excludePaths, bool useFtp, bool preservePermissionsAndTimestamps, string userFtp, string passwordFtp)
        {
            var sourceDirectory = new DirectoryInfo(sourceDir);
            var targetDirectory = new DirectoryInfo(targetDir);

            var sourceFiles = sourceDirectory.GetFiles();
            var targetFiles = targetDirectory.GetFiles();
            var sourceDirectories = sourceDirectory.GetDirectories();
            var targetDirectories = targetDirectory.GetDirectories();

            var totalOperations = sourceFiles.Length + targetFiles.Length + sourceDirectories.Length + targetDirectories.Length;

            using var progressBar = new ProgressBar();
            int progress = 0;

            void ReportProgress() => progressBar.Report((double)progress / totalOperations);

            async Task SynchronizeFile(FileInfo file, string destinationPath, bool isSourceToTarget)
            {
                if (excludePaths.Any(e => file.FullName.Contains(e, StringComparison.OrdinalIgnoreCase)))
                {
                    if (verbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"Excluding file: {file.FullName}");
                        Console.ResetColor();
                    }

                    if (logToFile)
                    {
                        LogMessage("Success", $"Excluding file: {file.FullName}", verbose, logToFile);
                    }
                    return;
                }

                if (!File.Exists(destinationPath) || file.LastWriteTime > File.GetLastWriteTime(destinationPath))
                {
                    var startTime = DateTime.Now;
                    file.CopyTo(destinationPath, true);
                    if (preservePermissionsAndTimestamps)
                    {
                        File.SetAttributes(destinationPath, file.Attributes);
                        File.SetLastWriteTime(destinationPath, file.LastWriteTime);
                    }
                    var endTime = DateTime.Now;

                    if (verbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"File synchronized: {file.FullName} -> {destinationPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)");
                        Console.ResetColor();
                    }

                    if (logToFile)
                    {
                        LogMessage("Success", $"File synchronized: {file.FullName} -> {destinationPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                    }

                    if (useFtp)
                    {
                        await UploadFileToFtp(file.Name, destinationPath, "ftpServerUrl", userFtp, passwordFtp);
                    }
                }

                progress++;
                ReportProgress();
            }

            async Task SynchronizeDirectory(DirectoryInfo directory, string destinationPath, bool isSourceToTarget)
            {
                if (excludePaths.Any(e => directory.FullName.Contains(e, StringComparison.OrdinalIgnoreCase)))
                {
                    if (verbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"Excluding directory: {directory.FullName}");
                        Console.ResetColor();
                    }

                    if (logToFile)
                    {
                        LogMessage("Success", $"Excluding directory: {directory.FullName}", verbose, logToFile);
                    }
                    return;
                }

                if (!Directory.Exists(destinationPath))
                {
                    var startTime = DateTime.Now;
                    Directory.CreateDirectory(destinationPath);
                    var endTime = DateTime.Now;

                    if (verbose)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Directory created: {destinationPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)");
                        Console.ResetColor();
                    }

                    if (logToFile)
                    {
                        LogMessage("Success", $"Directory created: {destinationPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                    }
                }

                await SynchronizeDirectories(directory.FullName, destinationPath, verbose, logToFile, exclude, excludePaths, useFtp, preservePermissionsAndTimestamps, userFtp, passwordFtp);
                progress++;
                ReportProgress();
            }

            foreach (var file in sourceFiles)
            {
                await SynchronizeFile(file, Path.Combine(targetDir, file.Name), true);
            }

            foreach (var file in targetFiles)
            {
                await SynchronizeFile(file, Path.Combine(sourceDir, file.Name), false);
            }

            foreach (var directory in sourceDirectories)
            {
                await SynchronizeDirectory(directory, Path.Combine(targetDir, directory.Name), true);
            }

            foreach (var directory in targetDirectories)
            {
                await SynchronizeDirectory(directory, Path.Combine(sourceDir, directory.Name), false);
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
