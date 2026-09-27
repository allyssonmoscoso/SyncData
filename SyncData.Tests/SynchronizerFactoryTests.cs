using System;
using SyncData.Configuration;
using SyncData.Synchronization;
using SyncData.Test.TestDoubles;
using Xunit;

namespace SyncData.Test
{
    public class SynchronizerFactoryTests
    {
        private readonly SynchronizerFactory _factory = new SynchronizerFactory();

        [Fact]
        public void CreateSynchronizer_WithoutFtp_ReturnsBidirectionalSynchronizer()
        {
            var synchronizer = _factory.CreateSynchronizer(new SyncConfiguration(), new FakeLogger());

            Assert.IsType<BidirectionalSynchronizer>(synchronizer);
        }

        [Fact]
        public void CreateSynchronizer_WithFtp_ThrowsNotImplementedException()
        {
            var config = new SyncConfiguration { UseFtp = true };

            Assert.Throws<NotImplementedException>(() => _factory.CreateSynchronizer(config, new FakeLogger()));
        }
    }
}
