using System;
using SyncData.Configuration;
using SyncData.Logging;

namespace SyncData.Synchronization
{
    /// <summary>
    /// Factory for creating file synchronizers based on configuration
    /// Uses Factory Pattern for object creation
    /// </summary>
    public class SynchronizerFactory
    {
        public FileSynchronizer CreateSynchronizer(SyncConfiguration config, Logger logger, IProgress<double>? progressReporter = null)
        {
            // Currently only one type, but this pattern allows easy extension
            // For example: UnidirectionalSynchronizer, FtpSynchronizer, etc.
            return new BidirectionalSynchronizer(config, logger, progressReporter);
        }
    }
}
