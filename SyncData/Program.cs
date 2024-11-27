﻿using System;
using System.IO;

class SyncData
{
    public static void Main(string[] args)
    {
        bool verbose = args.Contains("-v") || args.Contains("-verbose");
        bool logToFile = args.Contains("-log-file");
        string path1 = null;
        string path2 = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-v" || args[i] == "-verbose")
            {
                verbose = true;
            }
            else if (args[i] == "-log-file")
            {
                logToFile = true;
            }
            else if (path1 == null)
            {
                path1 = args[i];
            }
            else if (path2 == null)
            {
                path2 = args[i];
            }
        }

        if (path1 == null || path2 == null)
        {
            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Debe proporcionar dos rutas de directorio como argumentos.");
                Console.ResetColor();
            }

            LogMessage("Error", "Debe proporcionar dos rutas de directorio como argumentos.", verbose, logToFile);
            return;
        }

        if (path1 == path2)
        {
            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("La segunda ruta no puede ser igual a la primera.");
                Console.ResetColor();
            }

            LogMessage("Error", "La segunda ruta no puede ser igual a la primera.", verbose, logToFile);
            return;
        }

        if (Directory.Exists(path1) && Directory.Exists(path2))
        {
            SynchronizeDirectories(path1, path2, verbose, logToFile);
            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Sincronización completada.");
                Console.ResetColor();
            }

            LogMessage("Success", "Sincronización completada.", verbose, logToFile);
        }
        else
        {
            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Una o ambas rutas no existen.");
                Console.ResetColor();
            }

            LogMessage("Error", "Una o ambas rutas no existen.", verbose, logToFile);
        }

        static void LogMessage(string status, string message, bool verbose, bool logToFile)
        {
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}:{status}:{message}";
            if (logToFile)
            {
                File.AppendAllText("syncData.log", logMessage + Environment.NewLine);
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

                // Sincronizar archivos del directorio fuente con el de destino
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
                            Console.WriteLine($"Archivo sincronizado: {file.FullName} -> {targetFilePath} (Tiempo: {(endTime - startTime).TotalMilliseconds} ms)");
                        Console.ResetColor();
                    }
                         LogMessage("Success", $"Archivo sincronizado: {file.FullName} -> {targetFilePath} (Tiempo: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                    }

                    progress++;
                    progressBar.Report((double)progress / totalOperations);
                }

                // Sincronizar archivos del directorio destino con el de fuente
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
                            Console.WriteLine($"Archivo sincronizado: {file.FullName} -> {sourceFilePath} (Tiempo: {(endTime - startTime).TotalMilliseconds} ms)");
                        Console.ResetColor();
                    }
                         LogMessage("Success", $"Archivo sincronizado: {file.FullName} -> {sourceFilePath} (Tiempo: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                    }

                    progress++;
                    progressBar.Report((double)progress / totalOperations);
                }

                // Sincronizar subdirectorios
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
                            Console.WriteLine($"Directorio creado: {targetSubDirPath} (Tiempo: {(endTime - startTime).TotalMilliseconds} ms)");
                        Console.ResetColor();
                    }
                         LogMessage("Success", $"Directorio creado: {targetSubDirPath} (Tiempo: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
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
                            Console.WriteLine($"Directorio creado: {sourceSubDirPath} (Tiempo: {(endTime - startTime).TotalMilliseconds} ms)");
                        Console.ResetColor();
                    }
                         LogMessage("Success", $"Directorio creado: {sourceSubDirPath} (Tiempo: {(endTime - startTime).TotalMilliseconds} ms)", verbose, logToFile);
                    }

                     SynchronizeDirectories(directory.FullName, sourceSubDirPath, verbose, logToFile);
                    progress++;
                    progressBar.Report(1.0);
                    Console.WriteLine();
                }
            }
        }
    }

    public class ProgressBar : IDisposable, IProgress<double>
    {
        private const int blockCount = 10;
        private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private const string animation = @"|/-\";
        private int animationIndex = 0;

        private double currentProgress = 0;
        private string currentText = string.Empty;
        private bool disposed = false;
        private Timer timer;

        public ProgressBar()
        {
            timer = new Timer(TimerHandler);
            timer.Change(animationInterval, animationInterval);
        }

        public void Report(double value)
        {
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref currentProgress, value);
            var progressBlockCount = (int)(value * blockCount);
            var percent = (int)(value * 100);
            var text = string.Format("[{0}{1}] {2,3}% {3}",
                new string('#', progressBlockCount), new string('-', blockCount - progressBlockCount),
                percent,
                animation[animationIndex++ % animation.Length]);
            UpdateText(text);
        }

        private void TimerHandler(object state)
        {
            lock (timer)
            {
                if (disposed) return;
                animationIndex++;
                UpdateText(currentText);
            }
        }

        private void UpdateText(string text)
        {
            currentText = text;
            Console.Write('\r' + text + new string(' ', Console.WindowWidth - text.Length));
        }

        public void Dispose()
        {
            lock (timer)
            {
                disposed = true;
                UpdateText(string.Empty);
            }
        }
    }
}