using System;
using System.IO;
using SyncData.Logging;
using Xunit;

namespace SyncData.Test.Logging
{
    public class ConsoleLoggerTests
    {
        [Fact]
        public void Log_NonErrorWithoutVerbose_IsSuppressed()
        {
            var output = Capture(() => new ConsoleLogger(false).LogSuccess("done"));

            Assert.Equal(string.Empty, output.Trim());
        }

        [Fact]
        public void Log_ErrorWithoutVerbose_IsWritten()
        {
            var output = Capture(() => new ConsoleLogger(false).LogError("boom"));

            Assert.Contains("Error: boom", output);
        }

        [Fact]
        public void Log_WithVerbose_WritesEverything()
        {
            var output = Capture(() =>
            {
                var logger = new ConsoleLogger(true);
                logger.LogSuccess("done");
                logger.LogInfo("info");
                logger.LogError("boom");
            });

            Assert.Contains("Success: done", output);
            Assert.Contains("Info: info", output);
            Assert.Contains("Error: boom", output);
        }

        private static string Capture(Action action)
        {
            var original = Console.Out;
            var writer = new StringWriter();
            Console.SetOut(writer);
            try
            {
                action();
            }
            finally
            {
                Console.SetOut(original);
            }

            return writer.ToString();
        }
    }
}
