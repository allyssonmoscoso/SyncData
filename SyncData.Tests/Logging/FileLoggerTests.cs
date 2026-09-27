using System.IO;
using SyncData.Logging;
using SyncData.Test.TestDoubles;
using Xunit;

namespace SyncData.Test.Logging
{
    public class FileLoggerTests
    {
        [Fact]
        public void Log_WritesFormattedLine()
        {
            using var dir = new TempDirectory();
            var path = Path.Combine(dir.Path, "log.txt");
            var logger = new FileLogger(path);

            logger.LogSuccess("hello");

            var content = File.ReadAllText(path);
            Assert.Contains("Success:hello", content);
        }

        [Fact]
        public void Log_AppendsMultipleEntries()
        {
            using var dir = new TempDirectory();
            var path = Path.Combine(dir.Path, "log.txt");
            var logger = new FileLogger(path);

            logger.LogSuccess("first");
            logger.LogError("second");

            var lines = File.ReadAllLines(path);
            Assert.Equal(2, lines.Length);
            Assert.Contains("first", lines[0]);
            Assert.Contains("second", lines[1]);
        }
    }
}
