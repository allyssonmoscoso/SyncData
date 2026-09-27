using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SyncData.Configuration;
using SyncData.Synchronization;
using SyncData.Test.TestDoubles;
using Xunit;

namespace SyncData.Test
{
    public class BidirectionalSynchronizerTests
    {
        private static Task RunSyncAsync(SyncConfiguration config, FakeLogger logger, ProgressCollector progress)
        {
            var synchronizer = new BidirectionalSynchronizer(config, logger, progress);
            return synchronizer.SynchronizeAsync();
        }

        private static SyncConfiguration ConfigFor(string source, string target)
        {
            return new SyncConfiguration { SourcePath = source, TargetPath = target };
        }

        [Fact]
        public async Task SynchronizeAsync_CopiesFileFromSourceToTarget()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            source.CreateFile("a.txt", "hello");

            await RunSyncAsync(ConfigFor(source.Path, target.Path), new FakeLogger(), new ProgressCollector());

            var targetFile = Path.Combine(target.Path, "a.txt");
            Assert.True(File.Exists(targetFile));
            Assert.Equal("hello", File.ReadAllText(targetFile));
        }

        [Fact]
        public async Task SynchronizeAsync_CopiesFileFromTargetToSource()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            target.CreateFile("b.txt", "world");

            await RunSyncAsync(ConfigFor(source.Path, target.Path), new FakeLogger(), new ProgressCollector());

            var sourceFile = Path.Combine(source.Path, "b.txt");
            Assert.True(File.Exists(sourceFile));
            Assert.Equal("world", File.ReadAllText(sourceFile));
        }

        [Fact]
        public async Task SynchronizeAsync_CreatesTargetDirectoryWhenMissing()
        {
            using var source = new TempDirectory();
            var targetPath = TempDirectory.GetUniquePath();
            source.CreateFile("a.txt", "hello");

            try
            {
                await RunSyncAsync(ConfigFor(source.Path, targetPath), new FakeLogger(), new ProgressCollector());

                Assert.True(Directory.Exists(targetPath));
                Assert.True(File.Exists(Path.Combine(targetPath, "a.txt")));
            }
            finally
            {
                if (Directory.Exists(targetPath))
                {
                    Directory.Delete(targetPath, true);
                }
            }
        }

        [Fact]
        public async Task SynchronizeAsync_CreatesNestedSubdirectories()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            source.CreateFile(Path.Combine("sub", "inner.txt"), "nested");

            await RunSyncAsync(ConfigFor(source.Path, target.Path), new FakeLogger(), new ProgressCollector());

            var nested = Path.Combine(target.Path, "sub", "inner.txt");
            Assert.True(File.Exists(nested));
            Assert.Equal("nested", File.ReadAllText(nested));
        }

        [Fact]
        public async Task SynchronizeAsync_SkipsExcludedFile()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            source.CreateFile("keep.txt", "k");
            source.CreateFile("skip.txt", "s");

            var config = ConfigFor(source.Path, target.Path);
            config.Exclude = true;
            config.ExcludePaths.Add("skip.txt");

            await RunSyncAsync(config, new FakeLogger(), new ProgressCollector());

            Assert.True(File.Exists(Path.Combine(target.Path, "keep.txt")));
            Assert.False(File.Exists(Path.Combine(target.Path, "skip.txt")));
        }

        [Fact]
        public async Task SynchronizeAsync_SkipsExcludedDirectory()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            source.CreateFile(Path.Combine("sub", "inner.txt"), "nested");

            var config = ConfigFor(source.Path, target.Path);
            config.Exclude = true;
            config.ExcludePaths.Add("sub");

            await RunSyncAsync(config, new FakeLogger(), new ProgressCollector());

            Assert.False(Directory.Exists(Path.Combine(target.Path, "sub")));
        }

        [Fact]
        public async Task SynchronizeAsync_OverwritesWhenSourceIsNewer()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            var sourceFile = source.CreateFile("a.txt", "new");
            var targetFile = target.CreateFile("a.txt", "old");
            File.SetLastWriteTime(targetFile, DateTime.Now.AddMinutes(-10));
            File.SetLastWriteTime(sourceFile, DateTime.Now);

            await RunSyncAsync(ConfigFor(source.Path, target.Path), new FakeLogger(), new ProgressCollector());

            Assert.Equal("new", File.ReadAllText(Path.Combine(target.Path, "a.txt")));
        }

        [Fact]
        public async Task SynchronizeAsync_KeepsTargetWhenTargetIsNewer()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            var sourceFile = source.CreateFile("a.txt", "old");
            var targetFile = target.CreateFile("a.txt", "new");
            File.SetLastWriteTime(sourceFile, DateTime.Now.AddMinutes(-10));
            File.SetLastWriteTime(targetFile, DateTime.Now);

            await RunSyncAsync(ConfigFor(source.Path, target.Path), new FakeLogger(), new ProgressCollector());

            Assert.Equal("new", File.ReadAllText(Path.Combine(target.Path, "a.txt")));
        }

        [Fact]
        public async Task SynchronizeAsync_PreservesLastWriteTimeWhenEnabled()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            var sourceFile = source.CreateFile("a.txt", "data");
            var expected = new DateTime(2020, 1, 2, 3, 4, 5, DateTimeKind.Local);
            File.SetLastWriteTime(sourceFile, expected);

            var config = ConfigFor(source.Path, target.Path);
            config.PreservePermissionsAndTimestamps = true;

            await RunSyncAsync(config, new FakeLogger(), new ProgressCollector());

            var actual = File.GetLastWriteTime(Path.Combine(target.Path, "a.txt"));
            Assert.Equal(expected, actual, TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task SynchronizeAsync_ReportsProgress()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            source.CreateFile("a.txt", "x");

            var progress = new ProgressCollector();
            await RunSyncAsync(ConfigFor(source.Path, target.Path), new FakeLogger(), progress);

            Assert.NotEmpty(progress.Values);
            Assert.All(progress.Values, value => Assert.InRange(value, 0, 1));
        }

        [Fact]
        public async Task SynchronizeAsync_LogsSuccessWhenCopying()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            source.CreateFile("a.txt", "x");

            var logger = new FakeLogger();
            await RunSyncAsync(ConfigFor(source.Path, target.Path), logger, new ProgressCollector());

            Assert.True(logger.Contains("File synchronized"));
        }

        [Fact]
        public async Task SynchronizeAsync_WithCancelledToken_ThrowsOperationCanceled()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            source.CreateFile("a.txt", "x");

            var synchronizer = new BidirectionalSynchronizer(ConfigFor(source.Path, target.Path), new FakeLogger());
            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => synchronizer.SynchronizeAsync(cts.Token));
        }
    }
}
