using System.IO;
using SyncData.Configuration;
using SyncData.Logging;

namespace SyncData.Validation
{
    /// <summary>
    /// Validates synchronization configuration
    /// </summary>
    public class ConfigurationValidator
    {
        private readonly Logger _logger;

        public ConfigurationValidator(Logger logger)
        {
            _logger = logger;
        }

        public bool Validate(SyncConfiguration config)
        {
            if (!config.IsValid())
            {
                _logger.LogError("You must provide two directory paths as arguments.");
                return false;
            }

            if (config.Exclude && config.ExcludePaths.Count == 0)
            {
                _logger.LogError("-exclude: You must provide at least one exclude path as an argument.");
                return false;
            }

            if (config.HasSamePaths())
            {
                _logger.LogError("The second path cannot be the same as the first.");
                return false;
            }

            if (!Directory.Exists(config.SourcePath) || !Directory.Exists(config.TargetPath))
            {
                _logger.LogError("One or both paths do not exist.");
                return false;
            }

            return true;
        }
    }
}
