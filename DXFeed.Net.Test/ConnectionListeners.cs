using FluentAssertions;
using FluentAssertions.Extension.Json;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DXFeed.Net.Test
{
    public class ConnectionListeners
    {
        [Fact]
        public void Subscribe()
        {
            var listener = new Mock<IDXFeedConnectionListener>().Object;
            var collection = new DXFeedConnectionListenerCollection();
            
            
            collection.SubscribeListener(listener);

            collection.Count.Should().Be(1);
            collection.IsSubscribed(listener).Should().BeTrue();
        }

        [Fact]
        public void Unsubscribe()
        {
            var listener1 = new Mock<IDXFeedConnectionListener>().Object;
            var listener2 = new Mock<IDXFeedConnectionListener>().Object;
            var collection = new DXFeedConnectionListenerCollection();


            collection.SubscribeListener(listener1);
            collection.SubscribeListener(listener2);

            collection.Count.Should().Be(2);

            collection.UnsubscribeListener(listener1);

            collection.Count.Should().Be(1);
            collection.IsSubscribed(listener1).Should().BeFalse();
            collection.IsSubscribed(listener2).Should().BeTrue();
        }

        [Fact]
        public void RaceSubscribe()
        {
            int ready = 0;
            int one = 2000;
            int total = 0;

            var collection = new DXFeedConnectionListenerCollection();

            void x()
            {
                ++ready;
                while (ready != 2) Thread.Yield(); //sync start
                for (int i = 0; i < one; i++)
                {
                    var listener = new Mock<IDXFeedConnectionListener>().Object;
                    collection.SubscribeListener(listener);
                    if (i % 2 == 0)
                        collection.UnsubscribeListener(listener);
                    Interlocked.Increment(ref total);
                }
            }

            var t1 = Task.Run(x);
            var t2 = Task.Run(x);

            t1.Wait();
            t2.Wait();

            total.Should().Be(one * 2);
            collection.Count.Should().Be(one);
        }

        [Fact]
        public async Task InvokeStatus()
        {
            var thisTid = Environment.CurrentManagedThreadId;
            var eventTid = 0;
            IDXFeedConnection connectionPassed = null;
            DXFeedConnectionState statusPassed = DXFeedConnectionState.Disconnected;

            var collection = new DXFeedConnectionListenerCollection();           
            var listener = new Mock<IDXFeedConnectionListener>();

            listener.Setup(x => x.OnStatusChanged(It.IsAny<IDXFeedConnection>(), It.IsAny<DXFeedConnectionState>()))
                .Callback<IDXFeedConnection, DXFeedConnectionState>((connection, status) =>
                {
                    eventTid = Environment.CurrentManagedThreadId;
                    connectionPassed = connection;
                    statusPassed = status;
                });

            var listenerObject = listener.Object;
            collection.SubscribeListener(listenerObject);

            var connection = new Mock<IDXFeedConnection>();
            connection.Setup(x => x.State).Returns(DXFeedConnectionState.ReadyToSubscribe);

            var connectionObject = connection.Object;

            await collection.CallStatusChange(connectionObject);

            eventTid.Should()
                .NotBe(0)
                .And.NotBe(thisTid);

            connectionPassed.Should()
                .BeSameAs(connectionObject);

            statusPassed.Should()
                .Be(DXFeedConnectionState.ReadyToSubscribe);
        }
    }
}
