using System.IO;
using System.Threading.Tasks;
using SyncData.Configuration;
using SyncData.Core;
using SyncData.Test.TestDoubles;
using Xunit;

namespace SyncData.Test
{
    public class SyncApplicationTests
    {
        [Fact]
        public async Task RunAsync_WithValidConfiguration_ReturnsTrueAndSynchronizes()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            source.CreateFile("a.txt", "hello");

            var config = new SyncConfiguration { SourcePath = source.Path, TargetPath = target.Path };
            var app = new SyncApplication(config, new FakeLogger());

            var result = await app.RunAsync();

            Assert.True(result);
            var targetFile = Path.Combine(target.Path, "a.txt");
            Assert.True(File.Exists(targetFile));
            Assert.Equal("hello", File.ReadAllText(targetFile));
        }

        [Fact]
        public async Task RunAsync_WithInvalidConfiguration_ReturnsFalse()
        {
            var config = new SyncConfiguration();
            var app = new SyncApplication(config, new FakeLogger());

            var result = await app.RunAsync();

            Assert.False(result);
        }

        [Fact]
        public async Task RunAsync_WhenSynchronizationFails_ReturnsFalseAndLogsError()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            var config = new SyncConfiguration
            {
                SourcePath = source.Path,
                TargetPath = target.Path,
                UseFtp = true
            };
            var logger = new FakeLogger();
            var app = new SyncApplication(config, logger);

            var result = await app.RunAsync();

            Assert.False(result);
            Assert.True(logger.HasStatus("Error"));
        }
    }
}
