using SyncData.Configuration;
using SyncData.Test.TestDoubles;
using SyncData.Validation;
using Xunit;

namespace SyncData.Test
{
    public class ConfigurationValidatorTests
    {
        private readonly FakeLogger _logger = new FakeLogger();
        private readonly ConfigurationValidator _validator;

        public ConfigurationValidatorTests()
        {
            _validator = new ConfigurationValidator(_logger);
        }

        [Fact]
        public void Validate_WithMissingPaths_ReturnsFalse()
        {
            var config = new SyncConfiguration();

            Assert.False(_validator.Validate(config));
            Assert.True(_logger.Contains("You must provide two directory paths"));
        }

        [Fact]
        public void Validate_ExcludeWithoutPaths_ReturnsFalse()
        {
            var config = new SyncConfiguration { SourcePath = "/a", TargetPath = "/b", Exclude = true };

            Assert.False(_validator.Validate(config));
            Assert.True(_logger.Contains("-exclude"));
        }

        [Fact]
        public void Validate_WithSamePaths_ReturnsFalse()
        {
            using var dir = new TempDirectory();
            var config = new SyncConfiguration { SourcePath = dir.Path, TargetPath = dir.Path };

            Assert.False(_validator.Validate(config));
            Assert.True(_logger.Contains("cannot be the same"));
        }

        [Fact]
        public void Validate_WithMissingSource_ReturnsFalse()
        {
            using var target = new TempDirectory();
            var config = new SyncConfiguration
            {
                SourcePath = TempDirectory.GetUniquePath(),
                TargetPath = target.Path
            };

            Assert.False(_validator.Validate(config));
            Assert.True(_logger.Contains("source path does not exist"));
        }

        [Fact]
        public void Validate_WithMissingTarget_ReturnsTrue()
        {
            using var source = new TempDirectory();
            var config = new SyncConfiguration
            {
                SourcePath = source.Path,
                TargetPath = TempDirectory.GetUniquePath()
            };

            Assert.True(_validator.Validate(config));
        }

        [Fact]
        public void Validate_WithValidConfiguration_ReturnsTrue()
        {
            using var source = new TempDirectory();
            using var target = new TempDirectory();
            var config = new SyncConfiguration { SourcePath = source.Path, TargetPath = target.Path };

            Assert.True(_validator.Validate(config));
        }
    }
}
