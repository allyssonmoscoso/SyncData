using System;
using System.IO;

class SyncData
{
    public static void Main()
    {
        Console.Write("Ingrese la primera ruta de directorio: ");
        string path1 = Console.ReadLine();

        Console.Write("Ingrese la segunda ruta de directorio: ");
        string path2 = Console.ReadLine();

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

        // Sincronizar archivos del directorio fuente con el de destino
        foreach (var file in sourceDirectory.GetFiles())
        {
            var targetFilePath = Path.Combine(targetDir, file.Name);
            if (!File.Exists(targetFilePath) || file.LastWriteTime > File.GetLastWriteTime(targetFilePath))
            {
                file.CopyTo(targetFilePath, true);
            }
        }

        // Sincronizar archivos del directorio destino con el de fuente
        foreach (var file in targetDirectory.GetFiles())
        {
            var sourceFilePath = Path.Combine(sourceDir, file.Name);
            if (!File.Exists(sourceFilePath) || file.LastWriteTime > File.GetLastWriteTime(sourceFilePath))
            {
                file.CopyTo(sourceFilePath, true);
            }
        }

        // Sincronizar subdirectorios
        foreach (var directory in sourceDirectory.GetDirectories())
        {
            var targetSubDirPath = Path.Combine(targetDir, directory.Name);
            if (!Directory.Exists(targetSubDirPath))
            {
                Directory.CreateDirectory(targetSubDirPath);
            }
            SynchronizeDirectories(directory.FullName, targetSubDirPath);
        }

        foreach (var directory in targetDirectory.GetDirectories())
        {
            var sourceSubDirPath = Path.Combine(sourceDir, directory.Name);
            if (!Directory.Exists(sourceSubDirPath))
            {
                Directory.CreateDirectory(sourceSubDirPath);
            }
            SynchronizeDirectories(directory.FullName, sourceSubDirPath);
        }
    }
}