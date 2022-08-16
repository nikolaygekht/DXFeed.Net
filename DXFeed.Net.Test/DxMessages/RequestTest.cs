using DXFeed.Net.DXFeedMessage;
using DXFeed.Net.Platform;
using FluentAssertions;
using FluentAssertions.Extension.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DXFeed.Net.Test.DxMessages
{
    public class RequestTest
    {
        [Fact]
        public void Heartbeat()
        {
            var message = new DXFeedMessageHeartbeat("myclient");
            message.MessageType.Should().Be(DXFeedMessageType.Heartbeat);

            var jsonText = message.ToMessage().SerializeToString();

            jsonText.Should().BeCorrectJson();
            var json = jsonText.AsJson();
            json.Should()
                .HaveStringProperty("channel", x => x == "/meta/connect")
                .And
                .HaveStringProperty("clientId", x => x == "myclient");
        }

        [Fact]
        public void Authorization()
        {
            var message = new DXFeedMessageAuthorize("mytoken");
            message.MessageType.Should().Be(DXFeedMessageType.Authorize);

            var jsonText = message.ToMessage().SerializeToString();

            jsonText.Should().BeCorrectJson();
            var json = jsonText.AsJson();
            json.Should()
                .HaveStringProperty("channel", x => x == "/meta/handshake")
                .And
                .HaveObjectProperty("ext")
                    .Which.Should()
                    .HaveStringProperty("com.devexperts.auth.AuthToken", x => x == "mytoken");
        }

        [Fact]
        public void SubscriptionRequest_Ok_OneSymbol()
        {
            var message = new DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode.Add, "AAPL", "myclient");
            message.MessageType.Should().Be(DXFeedMessageType.SubscribeForQuotes);

            var jsonText = message.ToMessage().SerializeToString();

            jsonText.Should().BeCorrectJson();
            var json = jsonText.AsJson();

            json.Should()
                .HaveStringProperty("channel", x => x == "/service/sub")
                .And
                .HaveStringProperty("clientId", x => x == "myclient")
                .And
                .HaveObjectProperty("data");

            var data = json.GetProperty("data");
            data.Should()
                .HaveObjectProperty("add");

            var add = data.GetProperty("add");
         
            add.Should()
                .HaveProperty("Quote")
                    .Which.Should().BeArray()
                           .And.HaveLength(1);

            var arr = add.GetProperty("Quote");
            arr[0].Should().BeStringMatching(x => x == "AAPL");
        }

        [Fact]
        public void SubscriptionRequest_Ok_MultipleeSymbols()
        {
            var message = new DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode.Add, new string[] { "AAPL", "MSFT", "GOOG" }, "myclient");
            message.MessageType.Should().Be(DXFeedMessageType.SubscribeForQuotes);

            var jsonText = message.ToMessage().SerializeToString();

            jsonText.Should().BeCorrectJson();
            var json = jsonText.AsJson();

            json.Should()
                .HaveStringProperty("channel", x => x == "/service/sub")
                .And
                .HaveStringProperty("clientId", x => x == "myclient")
                .And
                .HaveObjectProperty("data");

            var data = json.GetProperty("data");
            data.Should()
                .HaveObjectProperty("add");

            var add = data.GetProperty("add");

            add.Should()
                .HaveProperty("Quote")
                    .Which.Should().BeArray()
                           .And.HaveLength(3);

            var arr = add.GetProperty("Quote");
            arr[0].Should().BeStringMatching(x => x == "AAPL");
            arr[1].Should().BeStringMatching(x => x == "MSFT");
            arr[2].Should().BeStringMatching(x => x == "GOOG");
        }

        [Fact]
        public void SubscriptionRequest_Ok_Remove()
        {
            var message = new DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode.Remove, "AAPL", "myclient");
            message.MessageType.Should().Be(DXFeedMessageType.SubscribeForQuotes);

            var jsonText = message.ToMessage().SerializeToString();

            jsonText.Should().BeCorrectJson();
            var json = jsonText.AsJson();

            json.Should()
                .HaveStringProperty("channel", x => x == "/service/sub")
                .And
                .HaveStringProperty("clientId", x => x == "myclient")
                .And
                .HaveObjectProperty("data");

            var data = json.GetProperty("data");
            data.Should()
                .HaveObjectProperty("remove");

            var add = data.GetProperty("remove");

            add.Should()
                .HaveProperty("Quote")
                    .Which.Should().BeArray()
                           .And.HaveLength(1);

            var arr = add.GetProperty("Quote");
            arr[0].Should().BeStringMatching(x => x == "AAPL");
        }

        [Fact]
        public void SubscriptionRequest_Fail()
        {
            ((Action)(() => new DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode.Remove, "AAPL", "myclient"))).Should().NotThrow();

            ((Action)(() => new DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode.Remove, (string)null, "myclient")))
                .Should().Throw<ArgumentException>();
            
            ((Action)(() => new DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode.Remove, "", "myclient")))
                .Should().Throw<ArgumentException>();

            ((Action)(() => new DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode.Remove, "AAPL", null)))
                .Should().Throw<ArgumentException>();

            ((Action)(() => new DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode.Remove, "AAPL", "")))
                .Should().Throw<ArgumentException>();

            ((Action)(() => new DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode.Remove, (string[])null, "myclient")))
                 .Should().Throw<ArgumentException>();

            ((Action)(() => new DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode.Remove, new string[] { }, "myclient")))
                 .Should().Throw<ArgumentException>();
        }
    }
}
