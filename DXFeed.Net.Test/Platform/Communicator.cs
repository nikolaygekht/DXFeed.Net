using DXFeed.Net.Platform;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

#pragma warning disable CS0642, S1116, S1854

namespace DXFeed.Net.Test.Platform
{
    public class CommunicatorTest
    {
        [Fact]
        public void TransportDisposed()
        {
            var transport = new Mock<ITransport>();
            transport.Setup(x => x.State).Returns(TransportState.Open);
            
            bool disposed = false;
            transport.Setup(x => x.Dispose()).Callback(() => disposed = true);
            using (var communication = new Communicator(transport.Object, true));
            disposed.Should().BeTrue();
        }

        [Fact]
        public void TransportNotDisposed()
        {
            var transport = new Mock<ITransport>();
            transport.Setup(x => x.State).Returns(TransportState.Open);

            bool disposed = false;
            transport.Setup(x => x.Dispose()).Callback(() => disposed = true);
            using (var communication = new Communicator(transport.Object, false));
            disposed.Should().BeFalse();
        }

        [Fact]
        public void SenderEvents()
        {
            var transport = new Mock<ITransport>();
            transport.Setup(x => x.State).Returns(TransportState.Open);
            transport.Setup(x => x.Receive()).Returns((string)null);
            using var communication = new Communicator(transport.Object, true);
            bool started = false, stopped = false;
            communication.SenderStarted += (sender, args) => started = true;
            communication.SenderStopped += (sender, args) => stopped = true;

            
            communication.Start();

            Tools.Wait(() => started, 5000);
            started.Should().BeTrue();
            
            communication.Close();

            Tools.Wait(() => stopped, 5000);
            stopped.Should().BeTrue();
        }

        [Fact]
        public void ReceiverEvents()
        {
            var transport = new Mock<ITransport>();
            transport.Setup(x => x.State).Returns(TransportState.Open);
            transport.Setup(x => x.Receive()).Returns((string)null);
            using var communication = new Communicator(transport.Object, true);
            bool started = false, stopped = false;
            communication.ReceiverStarted += (sender, args) => started = true;
            communication.ReceiverStopped += (sender, args) => stopped = true;


            communication.Start();

            Tools.Wait(() => started, 5000);
            started.Should().BeTrue();

            communication.Close();

            Tools.Wait(() => stopped, 5000);
            stopped.Should().BeTrue();
        }

        [Fact]
        public void SenderRaiseException()
        {
            var transport = new Mock<ITransport>();
            transport.Setup(x => x.State).Returns(TransportState.Open);

            bool sent = false;
            bool stopped = false;
            Exception exception = null;

            transport.Setup(x => x.Send(It.Is<string>(s => s == "a")))
                .Callback(() => throw new ArgumentException("a"));

            transport.Setup(x => x.Send(It.Is<string>(s => s == "b")))
                .Callback(() => sent = true);

            using var communication = new Communicator(transport.Object, true);

            communication.SenderStopped += (sender, args) => stopped = true;
            communication.ExceptionRaised += (sender, args) => exception = args.Exception;
            communication.MessageSent += (sender, args) => sent = args.Message == "b";

            communication.Start();

            communication.Send("a");
            Tools.Wait(() => exception != null, 5000);

            exception.Should()
                .NotBeNull()
                .And.BeOfType<ArgumentException>()
                .And.Subject.As<ArgumentException>().Message.Should()
                    .Be("a");

            stopped.Should().BeFalse();

            communication.Send("b");
            Tools.Wait(() => sent, 5000);

            sent.Should().BeTrue();

            communication.Close();
        }

        [Fact]
        public void ReceiverRaiseException()
        {
            var transport = new Mock<ITransport>();
            transport.Setup(x => x.State).Returns(TransportState.Open);

            bool started = false;
            bool stopped = false;
            Exception exception = null;

            transport.Setup(x => x.Receive())
                .Callback(() =>
                {
                    throw new ArgumentException("a");
                });

            using var communication = new Communicator(transport.Object, true);

            communication.ReceiverStarted += (sender, args) => started = true;
            communication.ReceiverStopped += (sender, args) => stopped = true;
            communication.ExceptionRaised += (sender, args) => exception = args.Exception;

            communication.Start();
            Tools.Wait(() => started, 5000);
            started.Should().BeTrue();

            Tools.Wait(() => exception != null, 5000);
            exception.Should()
                .NotBeNull()
                .And.BeOfType<ArgumentException>()
                .And.Subject.As<ArgumentException>().Message.Should()
                    .Be("a");
            stopped.Should().BeFalse();

            communication.Close();
            Tools.Wait(() => stopped, 5000);
            stopped.Should().BeTrue();
        }

        [Fact]
        public void ReceiverDoesNotRaiseAtClose()
        {
            var transport = new Mock<ITransport>();
            transport.Setup(x => x.State).Returns(TransportState.Open);

            bool exiting = false;
            bool started = false;
            bool stopped = false;
            Exception exception = null;

            transport.Setup(x => x.Receive())
                .Callback(() => 
                {
                    while (!exiting) ;
                    throw new ArgumentException("a");
                });

            using var communication = new Communicator(transport.Object, true);

            communication.ReceiverStarted += (sender, args) => started = true;
            communication.ReceiverStopped += (sender, args) => stopped = true;
            communication.ExceptionRaised += (sender, args) => exception = args.Exception;

            communication.Start();
            Tools.Wait(() => started, 5000);
            started.Should().BeTrue();

            exiting = true;
            communication.Close();

            Tools.Wait(() => stopped, 5000);

            exception.Should().BeNull();
            stopped.Should().BeTrue();
        }

        [Fact]
        public void SenderSends()
        {
            var transport = new Mock<ITransport>();
            transport.Setup(x => x.State).Returns(TransportState.Open);

            var messages = new List<string>();
            var confirmations = new List<string>();
            transport.Setup(x => x.Send(It.IsAny<string>())).Callback<string>(s => messages.Add(s));


            using var communication = new Communicator(transport.Object, true);
            
            bool started = false;
            
            communication.SenderStarted += (sender, args) => started = true;
            communication.MessageSent += (sender, args) => confirmations.Add(args.Message);

            communication.Start();
            Tools.Wait(() => started, 5000);
            started.Should().BeTrue();

            communication.Send("a");
            Tools.Wait(() => confirmations.Count == 1, 5000);

            messages.Count.Should().Be(1);
            confirmations.Count.Should().Be(1);

            messages[0].Should().Be("a");
            confirmations[0].Should().Be("a");

            communication.Send("b");
            Tools.Wait(() => confirmations.Count == 2, 5000);

            messages.Count.Should().Be(2);
            confirmations.Count.Should().Be(2);

            messages[1].Should().Be("b");
            confirmations[1].Should().Be("b");
        }

        [Fact]
        public void ReceiverReceives()
        {
            var transport = new Mock<ITransport>();
            transport.Setup(x => x.State).Returns(TransportState.Open);

            string currentMessage = null;
            
            transport.Setup(x => x.Receive()).Returns(() => currentMessage);

            using var communication = new Communicator(transport.Object, true);

            bool started = false;
            string receivedMessage = null;
            communication.ReceiverStarted += (sender, args) => started = true;
            communication.MessageReceived += (sender, args) => receivedMessage = args.Message;

            communication.Start();

            Tools.Wait(() => started, 5000);
            started.Should().BeTrue();

            currentMessage = "a";
            Tools.Wait(() => receivedMessage == "a", 5000);
            receivedMessage.Should().Be("a");

            currentMessage = "b";
            Tools.Wait(() => receivedMessage == "b", 5000);
            receivedMessage.Should().Be("b");

            communication.Close();
        }       
    }
}
