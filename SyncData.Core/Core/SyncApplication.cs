using System;
using System.Threading;
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
        private readonly IProgress<double>? _progress;
        private readonly ConfigurationValidator _validator;
        private readonly SynchronizerFactory _synchronizerFactory;

        public SyncApplication(SyncConfiguration config, Logger logger, IProgress<double>? progress = null)
        {
            _config = config;
            _logger = logger;
            _progress = progress;
            _validator = new ConfigurationValidator(logger);
            _synchronizerFactory = new SynchronizerFactory();
        }

        public async Task<bool> RunAsync(CancellationToken cancellationToken = default)
        {
            if (!_validator.Validate(_config))
            {
                return false;
            }

            try
            {
                var synchronizer = _synchronizerFactory.CreateSynchronizer(_config, _logger, _progress);

                await synchronizer.SynchronizeAsync(cancellationToken);

                _logger.LogSuccess("Synchronization completed.");
                return true;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInfo("Synchronization cancelled.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Synchronization failed: {ex.Message}");
                return false;
            }
        }
    }
}
