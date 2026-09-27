using SyncData.Configuration;
using Xunit;

namespace SyncData.Test
{
    public class SyncConfigurationTests
    {
        [Fact]
        public void IsValid_WithBothPaths_ReturnsTrue()
        {
            var config = new SyncConfiguration { SourcePath = "/a", TargetPath = "/b" };

            Assert.True(config.IsValid());
        }

        [Theory]
        [InlineData("", "/b")]
        [InlineData("/a", "")]
        [InlineData("", "")]
        public void IsValid_WithMissingPath_ReturnsFalse(string source, string target)
        {
            var config = new SyncConfiguration { SourcePath = source, TargetPath = target };

            Assert.False(config.IsValid());
        }

        [Fact]
        public void HasSamePaths_IsCaseInsensitive()
        {
            var config = new SyncConfiguration { SourcePath = "/Tmp/A", TargetPath = "/tmp/a" };

            Assert.True(config.HasSamePaths());
        }

        [Fact]
        public void HasSamePaths_WithDifferentPaths_ReturnsFalse()
        {
            var config = new SyncConfiguration { SourcePath = "/a", TargetPath = "/b" };

            Assert.False(config.HasSamePaths());
        }
    }
}
