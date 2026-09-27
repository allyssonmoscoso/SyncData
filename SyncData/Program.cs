using System;
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

            // Create and run the application
            var app = new SyncApplication(config, logger);
            var success = await app.RunAsync();

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
