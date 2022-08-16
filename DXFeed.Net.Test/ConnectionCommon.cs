using DXFeed.Net.Platform;
using FluentAssertions;
using FluentAssertions.Extension.Json;
using Moq;
using System;
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

        [Fact]
        public void CantCreateDiagnosticConnectionWithRealCommunicator()
        {
            var transport = new Mock<ITransport>();
            var communicator = new Communicator(transport.Object, false);
            ((Action)(() => (new DXFeedConnection("token", "client", 3000, communicator, true)).Dispose()))
                .Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be("communicator");
        }

        [Fact]
        public void CantCreateDiagnosticConnectionWithNotActiveCommunicator()
        {
            var communicator = new Mock<ICommunicator>();
            communicator.Setup(c => c.Active).Returns(false);
            
            ((Action)(() => (new DXFeedConnection("token", "client", 3000, communicator.Object, true)).Dispose()))
                .Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be("communicator");
        }

        [Fact]
        public void CantCreateDiagnosticConnectionWithEmptyclientCommunicator()
        {
            var communicator = new Mock<ICommunicator>();
            communicator.Setup(c => c.Active).Returns(true);
            
            ((Action)(() => (new DXFeedConnection("token", "", 3000, communicator.Object, true)).Dispose()))
                .Should().Throw<ArgumentException>()
                .Which.ParamName.Should().Be("clientId");
        }

        [Fact]
        public void CanCreateDiagnosticConnectionDummyCommunicator()
        {
            var communicator = new Mock<ICommunicator>();
            communicator.Setup(c => c.Active).Returns(true);
            
            DXFeedConnection connection = null;
            try
            {
                ((Action)(() => connection = new DXFeedConnection("token", "client", 3000, communicator.Object, true)))
                .Should().NotThrow();
                connection.State.Should().Be(DXFeedConnectionState.ReadyToSubscribe);
            }
            finally
            {
                connection?.Dispose();
            }
            
            
                
        }
    }
}
