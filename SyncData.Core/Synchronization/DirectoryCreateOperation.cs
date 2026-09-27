using System;
using System.IO;
using System.Threading.Tasks;
using SyncData.Logging;

namespace SyncData.Synchronization
{
    /// <summary>
    /// Handles directory creation operations
    /// </summary>
    public class DirectoryCreateOperation : FileOperation
    {
        private readonly Logger _logger;

        public DirectoryCreateOperation(string targetPath, Logger logger) 
            : base(string.Empty, targetPath)
        {
            _logger = logger;
        }

        public override async Task ExecuteAsync()
        {
            if (Directory.Exists(TargetPath))
            {
                return;
            }

            var startTime = DateTime.Now;
            await Task.Run(() => Directory.CreateDirectory(TargetPath));
            var endTime = DateTime.Now;

            _logger.LogInfo($"Directory created: {TargetPath} (Time: {(endTime - startTime).TotalMilliseconds} ms)");
        }
    }
}
