using SyncData.Logging;
using SyncData.Test.TestDoubles;
using Xunit;

namespace SyncData.Test.Logging
{
    public class CompositeLoggerTests
    {
        [Fact]
        public void Log_ForwardsToAllChildren()
        {
            var first = new FakeLogger();
            var second = new FakeLogger();
            var composite = new CompositeLogger();
            composite.AddLogger(first);
            composite.AddLogger(second);

            composite.LogSuccess("hello");

            Assert.True(first.Contains("hello"));
            Assert.True(second.Contains("hello"));
            Assert.True(first.HasStatus("Success"));
        }

        [Fact]
        public void Log_WithoutChildren_DoesNotThrow()
        {
            var composite = new CompositeLogger();

            composite.LogError("no children");
        }
    }
}
