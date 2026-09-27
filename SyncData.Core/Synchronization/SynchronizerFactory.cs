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
        /// <summary>
        /// Creates a file synchronizer based on the configuration
        /// Currently supports bidirectional synchronization, but can be extended
        /// to support other types (e.g., unidirectional, FTP-based, etc.)
        /// </summary>
        public FileSynchronizer CreateSynchronizer(
            SyncConfiguration config, 
            Logger logger, 
            IProgress<double>? progressReporter = null)
        {
            // Factory pattern allows easy extension for different synchronizer types
            // Future implementations could check config.SyncType or other flags
            // to determine which synchronizer to create
            
            if (config.UseFtp)
            {
                // Future: return new FtpSynchronizer(config, logger, progressReporter);
                throw new NotImplementedException("FTP synchronization not yet implemented");
            }

            // Default to bidirectional synchronizer
            return new BidirectionalSynchronizer(config, logger, progressReporter);
        }
    }
}
