using System;
using System.IO;
using System.Threading.Tasks;
using SyncData.Logging;

namespace SyncData.Synchronization
{
    /// <summary>
    /// Handles file copy operations with optional permission preservation
    /// </summary>
    public class FileCopyOperation : FileOperation
    {
        private readonly bool _preserveAttributes;
        private readonly Logger _logger;

        public FileCopyOperation(string sourcePath, string targetPath, bool preserveAttributes, Logger logger) 
            : base(sourcePath, targetPath)
        {
            _preserveAttributes = preserveAttributes;
            _logger = logger;
        }

        public override async Task ExecuteAsync()
        {
            var sourceFile = new FileInfo(SourcePath);
            var targetFile = new FileInfo(TargetPath);

            if (!ShouldCopyFile(sourceFile, TargetPath))
            {
                return;
            }

            var startTime = DateTime.Now;
            await Task.Run(() => File.Copy(SourcePath, TargetPath, true));

            if (_preserveAttributes)
            {
                File.SetAttributes(TargetPath, sourceFile.Attributes);
                File.SetLastWriteTime(TargetPath, sourceFile.LastWriteTime);
            }

            var endTime = DateTime.Now;
            _logger.LogSuccess($"File synchronized: {SourcePath} -> {TargetPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)");
        }
    }
}
