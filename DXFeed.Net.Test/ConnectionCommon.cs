using DXFeed.Net.Platform;
using FluentAssertions;
using FluentAssertions.Extension.Json;
using Moq;
using Xunit;

namespace DXFeed.Net.Test
{
    public class ConnectionCommon
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DisposeCommunicatorAtDispose(bool dispose)
        {
            bool disposeInvoked = false;
            var communicator = new Mock<ICommunicator>();
            communicator.Setup(x => x.Dispose()).Callback(() => { disposeInvoked = true; });

            var connection = new DXFeedConnection("", communicator.Object, dispose);
            connection.Dispose();

            disposeInvoked.Should().Be(dispose);
        }
    }
}
