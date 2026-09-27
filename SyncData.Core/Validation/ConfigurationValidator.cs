using System.IO;
using SyncData.Configuration;
using SyncData.Core.Localization;
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
                _logger.LogError(CoreLocalizer.Get("Validator_PathsRequired"));
                return false;
            }

            if (config.Exclude && config.ExcludePaths.Count == 0)
            {
                _logger.LogError(CoreLocalizer.Get("Validator_ExcludeRequired"));
                return false;
            }

            if (config.HasSamePaths())
            {
                _logger.LogError(CoreLocalizer.Get("Validator_SamePaths"));
                return false;
            }

            if (!Directory.Exists(config.SourcePath))
            {
                _logger.LogError(CoreLocalizer.Get("Validator_SourceMissing"));
                return false;
            }

            return true;
        }
    }
}
