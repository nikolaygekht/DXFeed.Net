using DXFeed.Net.DXFeedMessage;
using DXFeed.Net.Message;
using DXFeed.Net.Platform;
using FluentAssertions;
using FluentAssertions.Extension.Json;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Collections.Generic;
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

        [Fact]
        public void QuoteResponse_Constructor()
        {
            var l = new List<string>()
            {
                "eventSymbol","eventTime","sequence","timeNanoPart","bidTime","bidExchangeCode","bidPrice","bidSize","askTime","askExchangeCode","askPrice","askSize"
            };
            var arr = "[\"AAPL\",0,0,0,1660674843000,\"Z\",173.08,1100.0,1660674844000,\"Y\",173.1,848.0]".DeserializeToMessage();

            arr.Should().BeAssignableTo<IMessageElementArray>()
                .Which.Length.Should().Be(l.Count);

            var message = new DXFeedResponseQuote(l, arr.As<IMessageElementArray>());

            message.FirstQuote.Symbol.Should().Be("AAPL");
            message.FirstQuote.BidTime.Should().Be(1660674843000L.FromDXFeed());
            message.FirstQuote.BidExchangeCode.Should().Be("Z");
            message.FirstQuote.Bid.Should().Be(173.08);
            message.FirstQuote.BidSize.Should().Be(1100.0);
            message.FirstQuote.AskTime.Should().Be(1660674844000L.FromDXFeed());
            message.FirstQuote.AskExchangeCode.Should().Be("Y");
            message.FirstQuote.Ask.Should().Be(173.1);
            message.FirstQuote.AskSize.Should().Be(848.0);

            message.Should().HaveCount(1);
            message.Count.Should().Be(1);
            message.FirstQuote.Should().BeSameAs(message[0]);
        }

        [Fact]
        public void QuoteResponse_ParseFirst()
        {
            var message = "{\"data\":[[\"Quote\",[\"eventSymbol\",\"eventTime\",\"sequence\",\"timeNanoPart\",\"bidTime\",\"bidExchangeCode\",\"bidPrice\",\"bidSize\",\"askTime\",\"askExchangeCode\",\"askPrice\",\"askSize\"]],[\"AAPL\",0,0,0,1660674843000,\"Z\",173.08,900.0,1660674843000,\"Z\",173.09,200.0]],\"channel\":\"/service/data\"}"
                .DeserializeToMessage().As<IMessageElementObject>();

            var response = DXFeedResponseParser.Parse(message);

            response.ResponseType.Should().Be(DXFeedResponseType.Quote);
            var quote = response.As<DXFeedResponseQuote>();

            quote.FirstQuote.Symbol.Should().Be("AAPL");
            quote.FirstQuote.Bid.Should().Be(173.08);
            quote.FirstQuote.Ask.Should().Be(173.09);
        }

        [Fact]
        public void QuoteResponse_ParseSecond()
        {
            var message = "{\"data\":[[\"Quote\",[\"eventSymbol\",\"eventTime\",\"sequence\",\"timeNanoPart\",\"bidTime\",\"bidExchangeCode\",\"bidPrice\",\"bidSize\",\"askTime\",\"askExchangeCode\",\"askPrice\",\"askSize\"]],[\"AAPL\",0,0,0,1660674843000,\"Z\",173.08,900.0,1660674843000,\"Z\",173.09,200.0]],\"channel\":\"/service/data\"}"
                .DeserializeToMessage().As<IMessageElementObject>();
            DXFeedResponseParser.Parse(message);

            message = "{\"data\":[\"Quote\",[\"AAPL\",0,0,0,1660674843000,\"Z\",173.08,900.0,1660674843000,\"Z\",173.09,200.0]],\"channel\":\"/service/data\"}"
                .DeserializeToMessage().As<IMessageElementObject>();

            var response = DXFeedResponseParser.Parse(message);

            response.ResponseType.Should().Be(DXFeedResponseType.Quote);
            var quote = response.As<DXFeedResponseQuote>();

            quote.FirstQuote.Symbol.Should().Be("AAPL");
            quote.FirstQuote.Bid.Should().Be(173.08);
            quote.FirstQuote.Ask.Should().Be(173.09);
        }

        [Fact]
        public void QuoteResponse_Multiple()
        {
            var message = "{\"data\":[[\"Quote\",[\"eventSymbol\",\"eventTime\",\"sequence\",\"timeNanoPart\",\"bidTime\",\"bidExchangeCode\",\"bidPrice\",\"bidSize\",\"askTime\",\"askExchangeCode\",\"askPrice\",\"askSize\"]],[\"AAPL\",0,0,0,1660674843000,\"Z\",173.08,900.0,1660674843000,\"Z\",173.09,200.0]],\"channel\":\"/service/data\"}"
                .DeserializeToMessage().As<IMessageElementObject>();
            DXFeedResponseParser.Parse(message);

            message = "{\"data\":[\"Quote\",[\"MSFT\",0,0,0,1660678331000,\"Z\",293.08,3.0,1660678330000,\"Z\",293.12,101.0,\"AAPL\",0,0,0,1660678331000,\"Z\",172.84,200.0,1660678331000,\"Z\",172.85,105.0]],\"channel\":\"/service/data\"}"
                .DeserializeToMessage().As<IMessageElementObject>();

            var response = DXFeedResponseParser.Parse(message);

            response.ResponseType.Should().Be(DXFeedResponseType.Quote);
            var quote = response.As<DXFeedResponseQuote>();

            quote.Count.Should().Be(2);
            quote[0].Symbol.Should().Be("MSFT");
            quote[0].Bid.Should().Be(293.08);
            quote[0].Ask.Should().Be(293.12);

            quote[1].Symbol.Should().Be("AAPL");
            quote[1].Bid.Should().Be(172.84);
            quote[1].Ask.Should().Be(172.85);
        }
    }
}
