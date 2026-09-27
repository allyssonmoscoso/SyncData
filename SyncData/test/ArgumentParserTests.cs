using SyncData.Configuration;
using Xunit;

namespace SyncData.Test
{
    public class ArgumentParserTests
    {
        private static SyncConfiguration Parse(params string[] args) => new ArgumentParser().Parse(args);

        [Fact]
        public void Parse_NoArguments_ReturnsDefaultConfiguration()
        {
            var config = Parse();

            Assert.Equal(string.Empty, config.SourcePath);
            Assert.Equal(string.Empty, config.TargetPath);
            Assert.False(config.Verbose);
            Assert.False(config.LogToFile);
            Assert.False(config.Exclude);
            Assert.False(config.UseFtp);
            Assert.False(config.PreservePermissionsAndTimestamps);
            Assert.Empty(config.ExcludePaths);
        }

        [Theory]
        [InlineData("-v")]
        [InlineData("-verbose")]
        public void Parse_VerboseFlag_SetsVerbose(string flag)
        {
            Assert.True(Parse(flag).Verbose);
        }

        [Fact]
        public void Parse_LogFileFlag_SetsLogToFile()
        {
            Assert.True(Parse("-log-file").LogToFile);
        }

        [Fact]
        public void Parse_FtpFlag_SetsUseFtp()
        {
            Assert.True(Parse("-ftp").UseFtp);
        }

        [Fact]
        public void Parse_PreserveFlag_SetsPreserve()
        {
            Assert.True(Parse("-preserve").PreservePermissionsAndTimestamps);
        }

        [Fact]
        public void Parse_SourceAndTarget_SetPaths()
        {
            var config = Parse("-source=/tmp/a", "-target=/tmp/b");

            Assert.Equal("/tmp/a", config.SourcePath);
            Assert.Equal("/tmp/b", config.TargetPath);
        }

        [Fact]
        public void Parse_ExcludeSinglePath_SetsExcludeAndPath()
        {
            var config = Parse("-exclude=a.txt");

            Assert.True(config.Exclude);
            Assert.Equal(new[] { "a.txt" }, config.ExcludePaths);
        }

        [Fact]
        public void Parse_ExcludeMultiplePaths_TrimsAndSplits()
        {
            var config = Parse("-exclude=a.txt, b.txt ,c.txt");

            Assert.Equal(new[] { "a.txt", "b.txt", "c.txt" }, config.ExcludePaths);
        }

        [Fact]
        public void Parse_ExcludeWithBraces_RemovesBraces()
        {
            var config = Parse("-exclude={a.txt,b.txt}");

            Assert.Equal(new[] { "a.txt", "b.txt" }, config.ExcludePaths);
        }

        [Fact]
        public void Parse_ExcludeEmpty_LeavesExcludePathsEmpty()
        {
            var config = Parse("-exclude=");

            Assert.True(config.Exclude);
            Assert.Empty(config.ExcludePaths);
        }

        [Fact]
        public void Parse_UnknownArgument_IsIgnored()
        {
            var config = Parse("--nope", "random");

            Assert.Equal(string.Empty, config.SourcePath);
            Assert.Equal(string.Empty, config.TargetPath);
        }

        [Fact]
        public void Parse_ArgumentsInAnyOrder_AreApplied()
        {
            var config = Parse("-target=/b", "-v", "-source=/a");

            Assert.Equal("/a", config.SourcePath);
            Assert.Equal("/b", config.TargetPath);
            Assert.True(config.Verbose);
        }
    }
}
