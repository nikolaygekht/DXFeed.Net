using DXFeed.Net.DXFeedMessage;
using DXFeed.Net.Platform;
using FluentAssertions;
using FluentAssertions.Extension.Json;
using Moq;
using Xunit;

namespace DXFeed.Net.Test
{
    public class SessionStateMachine
    {
        [Fact]
        public void StartConnectionIfCommunicatorRunsInitially()
        {           
            var communicator = new Mock<ICommunicator>();           
            communicator.Setup(x => x.Active).Returns(true);

            string sentMessage = null;           
            communicator.Setup(x => x.Send(It.IsAny<string>())).Callback<string>(message => { sentMessage = message; });

            using var connection = new DXFeedConnection("", communicator.Object, false);

            Tools.Wait(() => connection.State != DXFeedConnectionState.Disconnected, 5000);
            connection.State.Should().NotBe(DXFeedConnectionState.Disconnected);

            Tools.Wait(() => !string.IsNullOrEmpty(sentMessage), 5000);

            sentMessage.Should()
                .NotBeNull()
                .And.BeCorrectJson();

            var message = sentMessage.AsJson()[0]; 
            message.Should().HaveStringProperty("channel", x => x == "/meta/handshake");
        }

        [Fact]
        public void StartConnectionWhenCommunicatorStarts()
        {
            var communicator = new Mock<ICommunicator>();

            bool communicatorIsActive = false;
            communicator.Setup(x => x.Active).Returns(() => communicatorIsActive);

            string sentMessage = null;
            communicator.Setup(x => x.Send(It.IsAny<string>())).Callback<string>(message => { sentMessage = message; });
            
            using var connection = new DXFeedConnection("", communicator.Object, false);
            
            connection.State.Should().Be(DXFeedConnectionState.Disconnected);

            communicatorIsActive = true;
            communicator.Raise(x => x.ReceiverStarted -= null, new CommunicatorEventArgs(communicator.Object));

            Tools.Wait(() => connection.State == DXFeedConnectionState.Connecting, 5000);
            connection.State.Should().Be(DXFeedConnectionState.Connecting);
            
            Tools.Wait(() => !string.IsNullOrEmpty(sentMessage), 5000);
            sentMessage.Should()
                .NotBeNull()
                .And.BeCorrectJson();

            var message = sentMessage.AsJson()[0];
            message.Should().HaveStringProperty("channel", x => x == "/meta/handshake");
        }

        [Fact]
        public void TokenIsSentInAuthorization()
        {
            var communicator = new Mock<ICommunicator>();
            communicator.Setup(x => x.Active).Returns(true);

            string sentMessage = null;
            communicator.Setup(x => x.Send(It.IsAny<string>())).Callback<string>(message => { sentMessage = message; });

            using var connection = new DXFeedConnection("mytoken", communicator.Object, false);

            Tools.Wait(() => !string.IsNullOrEmpty(sentMessage), 5000);

            sentMessage.Should()
                .NotBeNull()
                .And.BeCorrectJson();

            var message = sentMessage.AsJson()[0];
            message.Should().HaveStringProperty("channel", x => x == "/meta/handshake");
            message.Should().HaveObjectProperty("ext")
                .Which.Should()
                    .HaveStringProperty("com.devexperts.auth.AuthToken", x => x == "mytoken");
        }

        [Fact]
        public void SentHeartbitAfterAuthorization()
        {
            var communicator = new Mock<ICommunicator>();
            communicator.Setup(x => x.Active).Returns(true);

            string sentMessage = null;
            communicator.Setup(x => x.Send(It.IsAny<string>())).Callback<string>(message => { sentMessage = message; });

            using var connection = new DXFeedConnection("mytoken", communicator.Object, false);

            Tools.Wait(() => connection.State == DXFeedConnectionState.Connecting, 5000);
            connection.State.Should().Be(DXFeedConnectionState.Connecting);

            sentMessage = null;
            communicator.Raise(x => x.MessageReceived -= null,
                new CommunicatorMessageEventArgs(communicator.Object, 
                                                 "[ { \"channel\" : \"/meta/handshake\", \"successful\" : true, \"clientId\" : \"myclient\" } ]"));

            Tools.Wait(() => !string.IsNullOrEmpty(sentMessage), 5000);

            sentMessage.Should()
                .NotBeNull()
                .And.BeCorrectJson();

            var message = sentMessage.AsJson()[0];
            message.Should().HaveStringProperty("channel", x => x == "/meta/connect");
            message.Should().HaveStringProperty("clientId", x => x == "myclient");

            connection.State.Should().Be(DXFeedConnectionState.Connecting);
        }

        [Fact]
        public void CompleteAuthorizationAsHeartbitResponseReceived()
        {
            var communicator = new Mock<ICommunicator>();
            communicator.Setup(x => x.Active).Returns(true);

            string sentMessage = null;
            communicator.Setup(x => x.Send(It.IsAny<string>())).Callback<string>(message => { sentMessage = message; });

            using var connection = new DXFeedConnection("mytoken", communicator.Object, false);

            Tools.Wait(() => connection.State == DXFeedConnectionState.Connecting, 5000);
            
            communicator.Raise(x => x.MessageReceived -= null,
                new CommunicatorMessageEventArgs(communicator.Object,
                                                 "[ { \"channel\" : \"/meta/handshake\", \"successful\" : true, \"clientId\" : \"myclient\" } ]"));

            Tools.Wait(() => !string.IsNullOrEmpty(sentMessage), 5000);

            communicator.Raise(x => x.MessageReceived -= null,
                new CommunicatorMessageEventArgs(communicator.Object,
                                                 "[ { \"channel\" : \"/meta/connect\", \"successful\" : true } ]"));

            Tools.Wait(() => connection.State == DXFeedConnectionState.ReadyToSubscribe, 5000);

            connection.State.Should().Be(DXFeedConnectionState.ReadyToSubscribe);
        }

        [Fact]
        public void UpdateTimeoutFromAuthorizationResponse()
        {
            var communicator = new Mock<ICommunicator>();
            communicator.Setup(x => x.Active).Returns(true);

            string sentMessage = null;
            communicator.Setup(x => x.Send(It.IsAny<string>())).Callback<string>(message => { sentMessage = message; });

            using var connection = new DXFeedConnection("mytoken", "myclient", 100, communicator.Object, false);
            connection.State.Should().Be(DXFeedConnectionState.ReadyToSubscribe);
            connection.HeartbitPeriod.Should().Be(100);
            sentMessage = null;

            communicator.Raise(x => x.MessageReceived -= null,
                new CommunicatorMessageEventArgs(communicator.Object,
                                                 "[ { \"channel\" : \"/meta/handshake\", \"successful\" : true, \"clientId\" : \"newClientId\", \"advice\" : { \"timeout\" : 200 } } ]"));

            Tools.Wait(() => connection.HeartbitPeriod == 200, 5000);
            connection.HeartbitPeriod.Should().Be(200);
        }


        [Fact]
        public void UpdateClientIdFromHeartbitResponse()
        {
            var communicator = new Mock<ICommunicator>();
            communicator.Setup(x => x.Active).Returns(true);

            string sentMessage = null;
            communicator.Setup(x => x.Send(It.IsAny<string>())).Callback<string>(message => { sentMessage = message; });

            using var connection = new DXFeedConnection("mytoken", "myclient", 100, communicator.Object, false);
            connection.State.Should().Be(DXFeedConnectionState.ReadyToSubscribe);
            connection.ClientId.Should().Be("myclient");
            sentMessage = null;

            communicator.Raise(x => x.MessageReceived -= null,
                new CommunicatorMessageEventArgs(communicator.Object,
                                                 "[ { \"channel\" : \"/meta/connect\", \"successful\" : true, \"clientId\" : \"newClientId\" } ]"));

            Tools.Wait(() => connection.ClientId == "newClientId", 5000);
            connection.ClientId.Should().Be("newClientId");
        }

        [Fact]
        public void SendingHeartbit()
        {
            var communicator = new Mock<ICommunicator>();
            communicator.Setup(x => x.Active).Returns(true);

            string sentMessage = null;
            communicator.Setup(x => x.Send(It.IsAny<string>())).Callback<string>(message => { sentMessage = message; });
            
            using var connection = new DXFeedConnection("mytoken", "myclient", 100, communicator.Object, false);
            connection.State.Should().Be(DXFeedConnectionState.ReadyToSubscribe);

            //check it once!
            sentMessage = null;
            Tools.Wait(() => !string.IsNullOrEmpty(sentMessage), 1000);

            sentMessage.Should()
                .NotBeNull()
                .And.BeCorrectJson();

            var message = sentMessage.AsJson()[0];
            message.Should().HaveStringProperty("channel", x => x == "/meta/connect");
            message.Should().HaveStringProperty("clientId", x => x == "myclient");

            //check it twice!
            sentMessage = null;
            Tools.Wait(() => !string.IsNullOrEmpty(sentMessage), 1000);

            sentMessage.Should()
                .NotBeNull()
                .And.BeCorrectJson();

            message = sentMessage.AsJson()[0];
            message.Should().HaveStringProperty("channel", x => x == "/meta/connect");
            message.Should().HaveStringProperty("clientId", x => x == "myclient");
        }
    }
}
