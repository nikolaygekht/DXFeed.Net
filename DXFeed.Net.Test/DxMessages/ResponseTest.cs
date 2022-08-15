using DXFeed.Net.DXFeedMessage;
using DXFeed.Net.Message;
using DXFeed.Net.Platform;
using FluentAssertions;
using FluentAssertions.Extension.Json;
using Xunit;

namespace DXFeed.Net.Test.DxMessages
{
    public class ResponseTest
    {
        [Fact]
        public void AuthorizeRespose_Successful_NoAdvice()
        {
            var message = "{\"minimumVersion\":\"1.0\",\"clientId\":\"13farleu4sxZ3b1guicVNd85b44\",\"supportedConnectionTypes\":[\"websocket\"],\"channel\":\"/meta/handshake\",\"version\":\"1.0\",\"successful\":true}"
                .DeserializeToMessage()
                .As<IMessageElementObject>();
            var response = DXFeedResponseParser.Parse(message);

            response.ResponseType.Should().Be(DXFeedResponseType.Authorize);
            response.Should().BeOfType<DXFeedResponseAuthorize>();

            var response1 = response.As<DXFeedResponseAuthorize>();
            response1.Successful.Should().BeTrue();
            response1.ClientId.Should().Be("13farleu4sxZ3b1guicVNd85b44");
            response1.Advice.Should().BeNull();
        }

        [Fact]
        public void AuthorizeRespose_Successful_WithAdvice()
        {
            var message = "{\"minimumVersion\":\"1.0\",\"clientId\":\"13farleu4sxZ3b1guicVNd85b44\",\"supportedConnectionTypes\":[\"websocket\"],\"advice\":{\"interval\":0,\"timeout\":30000,\"reconnect\":\"retry\"},\"channel\":\"/meta/handshake\",\"version\":\"1.0\",\"successful\":true}"
                .DeserializeToMessage()
                .As<IMessageElementObject>();
            var response = DXFeedResponseParser.Parse(message);

            response.ResponseType.Should().Be(DXFeedResponseType.Authorize);
            response.Should().BeOfType<DXFeedResponseAuthorize>();

            var response1 = response.As<DXFeedResponseAuthorize>();
            response1.Successful.Should().BeTrue();
            response1.ClientId.Should().Be("13farleu4sxZ3b1guicVNd85b44");
            response1.Advice.Should().NotBeNull();

            response1.Advice.Reconnect.Should().Be("retry");
            response1.Advice.Timeout.Should().Be(30000);
            response1.Advice.Interval.Should().Be(0);
        }

        [Fact]
        public void AuthorizeRespose_NotSuccessfull()
        {
            var message = "{\"minimumVersion\":\"1.0\",\"channel\":\"/meta/handshake\",\"version\":\"1.0\",\"successful\":false}"
                .DeserializeToMessage()
                .As<IMessageElementObject>();
            var response = DXFeedResponseParser.Parse(message);

            response.ResponseType.Should().Be(DXFeedResponseType.Authorize);
            response.Should().BeOfType<DXFeedResponseAuthorize>();

            var response1 = response.As<DXFeedResponseAuthorize>();
            response1.Successful.Should().BeFalse();
            response1.ClientId.Should().BeNull();
            response1.Advice.Should().BeNull();
        }
    }
}
