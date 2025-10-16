using System;
using System.IO;
using System.Threading.Tasks;

namespace SyncData.Synchronization
{
    /// <summary>
    /// Abstract base class for file operations
    /// </summary>
    public abstract class FileOperation
    {
        public string SourcePath { get; protected set; } = string.Empty;
        public string TargetPath { get; protected set; } = string.Empty;

        protected FileOperation(string sourcePath, string targetPath)
        {
            SourcePath = sourcePath;
            TargetPath = targetPath;
        }

        public abstract Task ExecuteAsync();
        
        protected bool ShouldCopyFile(FileInfo sourceFile, string targetFilePath)
        {
            return !File.Exists(targetFilePath) || 
                   sourceFile.LastWriteTime > File.GetLastWriteTime(targetFilePath);
        }
    }
}
