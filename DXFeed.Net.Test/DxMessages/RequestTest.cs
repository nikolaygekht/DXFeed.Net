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
    }
}
