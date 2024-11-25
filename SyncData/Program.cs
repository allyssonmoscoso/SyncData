using System;
using System.IO;

class SyncData
{
    public static void Main(string[] args)
    {
        if (args.Length != 2)
    {
            Console.WriteLine("Debe proporcionar dos rutas de directorio como argumentos.");
            return;
        }

        string path1 = args[0];
        string path2 = args[1];

            if (path1 == path2)
            {
            Console.WriteLine("La segunda ruta no puede ser igual a la primera.");
            return;
            }

        if (Directory.Exists(path1) && Directory.Exists(path2))
        {
            SynchronizeDirectories(path1, path2);
            Console.WriteLine("Sincronización completada.");
        }
        else
        {
            Console.WriteLine("Una o ambas rutas no existen.");
        }
    }

    private static void SynchronizeDirectories(string sourceDir, string targetDir)
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
                    Console.WriteLine(
                        $"Archivo sincronizado: {file.FullName} -> {targetFilePath} (Tiempo: {(endTime - startTime).TotalMilliseconds} ms)");
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
                    Console.WriteLine(
                        $"Archivo sincronizado: {file.FullName} -> {sourceFilePath} (Tiempo: {(endTime - startTime).TotalMilliseconds} ms)");
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
                    Console.WriteLine(
                        $"Directorio creado: {targetSubDirPath} (Tiempo: {(endTime - startTime).TotalMilliseconds} ms)");
                }

                SynchronizeDirectories(directory.FullName, targetSubDirPath);
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
                    Console.WriteLine(
                        $"Directorio creado: {sourceSubDirPath} (Tiempo: {(endTime - startTime).TotalMilliseconds} ms)");
                }

                SynchronizeDirectories(directory.FullName, sourceSubDirPath);
                progress++;
                progressBar.Report(1.0);
                Console.WriteLine();
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