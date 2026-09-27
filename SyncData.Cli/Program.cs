using System;
using System.Threading;
using System.Threading.Tasks;
using SyncData.Configuration;
using SyncData.Core;
using SyncData.Logging;

namespace SyncData
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            // Parse command-line arguments
            var parser = new ArgumentParser();
            var config = parser.Parse(args);

            // Create logger based on configuration
            var logger = CreateLogger(config);

            // Allow the user to stop a running synchronization with Ctrl+C
            using var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (_, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellationTokenSource.Cancel();
                logger.LogInfo("Cancellation requested. Stopping...");
            };

            // Create the console progress bar and run the application
            using var progressBar = new ProgressBar();
            var app = new SyncApplication(config, logger, progressBar);
            var success = await app.RunAsync(cancellationTokenSource.Token);

            // Propagate the result to the process exit code
            Environment.ExitCode = success ? 0 : 1;
        }

        private static Logger CreateLogger(SyncConfiguration config)
        {
            var compositeLogger = new CompositeLogger();

            // Always add console logger
            compositeLogger.AddLogger(new ConsoleLogger(config.Verbose));

            // Add file logger if requested
            if (config.LogToFile)
            {
                compositeLogger.AddLogger(new FileLogger());
            }

            return compositeLogger;
        }
    }
}
