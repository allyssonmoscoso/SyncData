using System;
using System.Threading.Tasks;
using SyncData.Configuration;
using SyncData.Logging;
using SyncData.Synchronization;
using SyncData.Validation;

namespace SyncData.Core
{
    /// <summary>
    /// Main application orchestrator that coordinates the synchronization process
    /// </summary>
    public class SyncApplication
    {
        private readonly SyncConfiguration _config;
        private readonly Logger _logger;
        private readonly ConfigurationValidator _validator;
        private readonly SynchronizerFactory _synchronizerFactory;

        public SyncApplication(SyncConfiguration config, Logger logger)
        {
            _config = config;
            _logger = logger;
            _validator = new ConfigurationValidator(logger);
            _synchronizerFactory = new SynchronizerFactory();
        }

        public async Task<bool> RunAsync()
        {
            if (!_validator.Validate(_config))
            {
                return false;
            }

            try
            {
                using var progressBar = new ProgressBar();
                var synchronizer = _synchronizerFactory.CreateSynchronizer(_config, _logger, progressBar);
                
                await synchronizer.SynchronizeAsync();
                
                _logger.LogSuccess("Synchronization completed.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Synchronization failed: {ex.Message}");
                return false;
            }
        }
    }
}
