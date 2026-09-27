using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SyncData.Synchronization;
using SyncData.Test.TestDoubles;
using Xunit;

namespace SyncData.Test
{
    public class FileOperationTests
    {
        [Fact]
        public async Task FileCopyOperation_WithCancelledToken_Throws()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            var file = source.CreateFile("a.txt", "x");
            var operation = new FileCopyOperation(file, Path.Combine(target.Path, "a.txt"), false, new FakeLogger());

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation.ExecuteAsync(cts.Token));
        }

        [Fact]
        public async Task DirectoryCreateOperation_WithCancelledToken_Throws()
        {
            using var target = new TempDirectory();
            var operation = new DirectoryCreateOperation(Path.Combine(target.Path, "new"), new FakeLogger());

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => operation.ExecuteAsync(cts.Token));
        }

        [Fact]
        public async Task FileCopyOperation_WithoutToken_CopiesFile()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            var file = source.CreateFile("a.txt", "content");
            var targetFile = Path.Combine(target.Path, "a.txt");
            var operation = new FileCopyOperation(file, targetFile, false, new FakeLogger());

            await operation.ExecuteAsync();

            Assert.True(File.Exists(targetFile));
            Assert.Equal("content", File.ReadAllText(targetFile));
        }
    }
}
