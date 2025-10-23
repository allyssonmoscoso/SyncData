using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SyncData.Configuration;
using SyncData.Logging;

namespace SyncData.Synchronization
{
    /// <summary>
    /// Synchronizes files and directories bidirectionally
    /// </summary>
    public class BidirectionalSynchronizer : FileSynchronizer
    {
        private int _progress;
        private int _totalOperations;

        public BidirectionalSynchronizer(SyncConfiguration config, Logger logger, IProgress<double>? progressReporter = null) 
            : base(config, logger, progressReporter)
        {
        }

        public override async Task SynchronizeAsync()
        {
            await SynchronizeDirectoriesAsync(Config.SourcePath, Config.TargetPath);
        }

        private async Task SynchronizeDirectoriesAsync(string sourceDir, string targetDir)
        {
            var sourceDirectory = new DirectoryInfo(sourceDir);
            var targetDirectory = new DirectoryInfo(targetDir);

            var filesToCopy = sourceDirectory.GetFiles().Length + targetDirectory.GetFiles().Length;
            var directoriesToCreate = sourceDirectory.GetDirectories().Length + targetDirectory.GetDirectories().Length;
            _totalOperations = filesToCopy + directoriesToCreate;
            _progress = 0;

            // Synchronize files from source to target
            await SynchronizeFilesAsync(sourceDirectory, targetDir);

            // Synchronize files from target to source
            await SynchronizeFilesAsync(targetDirectory, sourceDir);

            // Synchronize subdirectories from source to target
            await SynchronizeSubdirectoriesAsync(sourceDirectory, targetDir);

            // Synchronize subdirectories from target to source
            await SynchronizeSubdirectoriesAsync(targetDirectory, sourceDir);
        }

        private async Task SynchronizeFilesAsync(DirectoryInfo sourceDirectory, string targetDir)
        {
            foreach (var file in sourceDirectory.GetFiles())
            {
                if (IsExcluded(file.FullName))
                {
                    LogExcluded("file", file.FullName);
                    continue;
                }

                var targetFilePath = Path.Combine(targetDir, file.Name);
                var copyOperation = new FileCopyOperation(
                    file.FullName, 
                    targetFilePath, 
                    Config.PreservePermissionsAndTimestamps, 
                    Logger);

                await copyOperation.ExecuteAsync();

                UpdateProgress();
            }
        }

        private async Task SynchronizeSubdirectoriesAsync(DirectoryInfo sourceDirectory, string targetDir)
        {
            foreach (var directory in sourceDirectory.GetDirectories())
            {
                if (IsExcluded(directory.FullName))
                {
                    LogExcluded("directory", directory.FullName);
                    continue;
                }

                var targetSubDirPath = Path.Combine(targetDir, directory.Name);
                var createDirOperation = new DirectoryCreateOperation(targetSubDirPath, Logger);
                await createDirOperation.ExecuteAsync();

                await SynchronizeDirectoriesAsync(directory.FullName, targetSubDirPath);

                UpdateProgress();
            }
        }

        private void UpdateProgress()
        {
            _progress++;
            if (_totalOperations > 0)
            {
                ProgressReporter?.Report((double)_progress / _totalOperations);
            }
        }
    }
}
