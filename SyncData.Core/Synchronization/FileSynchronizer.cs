using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SyncData.Configuration;
using SyncData.Logging;

namespace SyncData.Synchronization
{
    /// <summary>
    /// Base class for file synchronization strategies
    /// </summary>
    public abstract class FileSynchronizer
    {
        protected readonly SyncConfiguration Config;
        protected readonly Logger Logger;
        protected readonly IProgress<double>? ProgressReporter;

        protected FileSynchronizer(SyncConfiguration config, Logger logger, IProgress<double>? progressReporter = null)
        {
            Config = config;
            Logger = logger;
            ProgressReporter = progressReporter;
        }

        public abstract Task SynchronizeAsync();

        protected bool IsExcluded(string path)
        {
            return Config.ExcludePaths.Any(e => path.Contains(e, StringComparison.OrdinalIgnoreCase));
        }

        protected void LogExcluded(string itemType, string path)
        {
            Logger.LogInfo($"Excluding {itemType}: {path}");
        }
    }
}
