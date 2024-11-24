using System;
using System.IO;

class SyncData
{
    public static void Main()
    {
        string path1;
        string path2;
        Console.WriteLine("Este programa sincroniza dos directorios, copiando los archivos que no existan en el directorio de destino o que hayan sido modificados en el directorio fuente.");

        Console.Write("Ingrese la primera ruta de directorio: ");
        path1 = Console.ReadLine();

        do
        {
        Console.Write("Ingrese la segunda ruta de directorio: ");
            path2 = Console.ReadLine();
            if (path1 == path2)
            {
                Console.WriteLine("La segunda ruta no puede ser igual a la primera. Intente nuevamente.");
            }
        } while (path1 == path2);

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
                Console.WriteLine($"Archivo sincronizado: {file.FullName} -> {targetFilePath}");
            }
        }

        // Sincronizar archivos del directorio destino con el de fuente
        foreach (var file in targetDirectory.GetFiles())
        {
            var sourceFilePath = Path.Combine(sourceDir, file.Name);
            if (!File.Exists(sourceFilePath) || file.LastWriteTime > File.GetLastWriteTime(sourceFilePath))
            {
                file.CopyTo(sourceFilePath, true);
                Console.WriteLine($"Archivo sincronizado: {file.FullName} -> {sourceFilePath}");
            }
        }

        // Sincronizar subdirectorios
        foreach (var directory in sourceDirectory.GetDirectories())
        {
            var targetSubDirPath = Path.Combine(targetDir, directory.Name);
            if (!Directory.Exists(targetSubDirPath))
            {
                Directory.CreateDirectory(targetSubDirPath);
                Console.WriteLine($"Directorio creado: {targetSubDirPath}");
            }
            SynchronizeDirectories(directory.FullName, targetSubDirPath);
        }

        foreach (var directory in targetDirectory.GetDirectories())
        {
            var sourceSubDirPath = Path.Combine(sourceDir, directory.Name);
            if (!Directory.Exists(sourceSubDirPath))
            {
                Directory.CreateDirectory(sourceSubDirPath);
                Console.WriteLine($"Directorio creado: {sourceSubDirPath}");
            }
            SynchronizeDirectories(directory.FullName, sourceSubDirPath);
        }
    }
}