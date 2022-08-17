using DXFeed.Net.DXFeedMessage;
using DXFeed.Net.Message;
using DXFeed.Net.Platform;
using FluentAssertions;
using FluentAssertions.Extension.Json;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using System.Collections.Generic;
using Xunit;
using static DXFeed.Net.DXFeedMessage.DXFeedResponseQuote;

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
        public void Candle_Constructor_AllDefined()
        {
            var l = new List<string>()
            {
                "eventSymbol", "eventTime", "eventFlags", "index", "time", "sequence", "count", "open", "high", "low", "close", "volume", "vwap", "bidVolume", "askVolume", "impVolatility", "openInterest"
            };
            var arr = "[ \"AAPL{=4h}\", 0, 4, 7106775869502259000, 1654675200000, 555, 99, 147.85, 148.53, 147.85, 148.45, 22006, 148.2886949495592, 10202, 11804, 0.3553, 0.123 ]".DeserializeToMessage();

            arr.Should().BeAssignableTo<IMessageElementArray>()
                .Which.Length.Should().Be(l.Count);

            var message = new DXFeedResponseCandle(l, arr.As<IMessageElementArray>());

            message.FirstCandle.Symbol.Should().Be("AAPL{=4h}");
            message.FirstCandle.Time.Should().Be(1654675200000L.FromDXFeed());
            message.FirstCandle.Index.Should().Be(7106775869502259000);
            message.FirstCandle.Sequence.Should().Be(555);
            message.FirstCandle.Count.Should().Be(99);
            message.FirstCandle.Open.Should().Be(147.85);
            message.FirstCandle.High.Should().Be(148.53);
            message.FirstCandle.Low.Should().Be(147.85);
            message.FirstCandle.Close.Should().Be(148.45);
            message.FirstCandle.Volume.Should().Be(22006);
            message.FirstCandle.BidVolume.Should().Be(10202);
            message.FirstCandle.AskVolume.Should().Be(11804);
            message.FirstCandle.VWAP.Should().BeApproximately(148.2886949495592, 1e-12);
            message.FirstCandle.Volatility.Should().Be(0.3553);
            message.FirstCandle.OpenInterest.Should().Be(0.123);
        }

        [Fact]
        public void Candle_Constructor_WithNans()
        {
            var l = new List<string>()
            {
                "eventSymbol", "eventTime", "eventFlags", "index", "time", "sequence", "count", "open", "high", "low", "close", "volume", "vwap", "bidVolume", "askVolume", "impVolatility", "openInterest"
            };
            var arr = "[ \"AAPL{=4h}\", 0, 11, 7106431471933456000, 1654595013709, 0, 0, \"NaN\", \"NaN\", \"NaN\", \"NaN\", \"NaN\", \"NaN\", \"NaN\", \"NaN\", \"NaN\", \"NaN\" ]".DeserializeToMessage();

            arr.Should().BeAssignableTo<IMessageElementArray>()
                .Which.Length.Should().Be(l.Count);

            var message = new DXFeedResponseCandle(l, arr.As<IMessageElementArray>());

            message.FirstCandle.Symbol.Should().Be("AAPL{=4h}");
            message.FirstCandle.Time.Should().Be(1654595013709L.FromDXFeed());
            message.FirstCandle.Index.Should().Be(7106431471933456000);
            message.FirstCandle.Sequence.Should().Be(0);
            message.FirstCandle.Count.Should().Be(0);
            message.FirstCandle.Open.Should().Be(double.NaN);
            message.FirstCandle.High.Should().Be(double.NaN);
            message.FirstCandle.Low.Should().Be(double.NaN);
            message.FirstCandle.Close.Should().Be(double.NaN);
            message.FirstCandle.Volume.Should().Be(double.NaN);
            message.FirstCandle.BidVolume.Should().Be(double.NaN);
            message.FirstCandle.AskVolume.Should().Be(double.NaN);
            message.FirstCandle.VWAP.Should().Be(double.NaN);
            message.FirstCandle.Volatility.Should().Be(double.NaN);
            message.FirstCandle.OpenInterest.Should().Be(double.NaN);
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

        [Fact]
        public void CandleResponse_ParseFirst()
        {
            var message = "{ \"data\": [ [ \"Candle\", [ \"eventSymbol\", \"eventTime\", \"eventFlags\", \"index\", \"time\", \"sequence\", \"count\", \"open\", \"high\", \"low\", \"close\", \"volume\", \"vwap\", \"bidVolume\", \"askVolume\", \"impVolatility\", \"openInterest\" ] ], [ \"AAPL{=4h}\", 0, 4, 7106775869502259000, 1654675200000, 0, 99, 147.85, 148.53, 147.85, 148.45, 22006, 148.2886949495592, 10202, 11804, 0.3553, 0 ] ], \"channel\": \"/service/timeSeriesData\" }"
                    .DeserializeToMessage().As<IMessageElementObject>();

            var response = DXFeedResponseParser.Parse(message);

            response.ResponseType.Should().Be(DXFeedResponseType.Candle);
            var candle = response.As<DXFeedResponseCandle>();

            candle.FirstCandle.Symbol.Should().Be("AAPL{=4h}");
            candle.FirstCandle.Time.Should().Be(1654675200000L.FromDXFeed());
            candle.FirstCandle.Open.Should().Be(147.85);
            candle.FirstCandle.Close.Should().Be(148.45);
            candle.FirstCandle.High.Should().Be(148.53);
            candle.FirstCandle.Low.Should().Be(147.85);
        }

        [Fact]
        public void CandleResponse_ParseSecond()
        {
            var message = "{ \"data\": [ [ \"Candle\", [ \"eventSymbol\", \"eventTime\", \"eventFlags\", \"index\", \"time\", \"sequence\", \"count\", \"open\", \"high\", \"low\", \"close\", \"volume\", \"vwap\", \"bidVolume\", \"askVolume\", \"impVolatility\", \"openInterest\" ] ], [ \"AAPL{=4h}\", 0, 4, 7106775869502259000, 1654675200000, 0, 99, 147.85, 148.53, 147.85, 148.45, 22006, 148.2886949495592, 10202, 11804, 0.3553, 0 ] ], \"channel\": \"/service/timeSeriesData\" }"
                    .DeserializeToMessage().As<IMessageElementObject>();

            DXFeedResponseParser.Parse(message);

            message = "{ \"data\": [ \"Candle\", [ \"AAPL{=4h}\", 0, 1, 7106590326915072000, 1654632000000, 0, 1027, 148.66, 148.71, 148.2, 148.22, 6639840, 148.6959981741126, 147134, 6492706, 0.3534, 0, \"AAPL{=4h}\", 0, 1, 7106528479386010000, 1654617600000, 0, 110285, 147.565, 149, 147.09, 148.69, 24672350, 148.3468091723488, 11391478, 13280872, 0.3511, 0, \"AAPL{=4h}\", 0, 1, 7106466631856947000, 1654603200000, 0, 118542, 144.17, 148.3, 143.8, 147.57, 27498775, 146.6560985844133, 12448651, 15050124, 0.3649, 0, \"AAPL{=4h}\", 0, 11, 7106431471933456000, 1654595013709, 0, 0, \"NaN\", \"NaN\", \"NaN\", \"NaN\", \"NaN\", \"NaN\", \"NaN\", \"NaN\", \"NaN\", \"NaN\" ] ], \"channel\": \"/service/timeSeriesData\" }"
                    .DeserializeToMessage().As<IMessageElementObject>();

            var response = DXFeedResponseParser.Parse(message);

            response.ResponseType.Should().Be(DXFeedResponseType.Candle);
            var candle = response.As<DXFeedResponseCandle>();
            candle.Count.Should().Be(4);
            candle.FirstCandle.Time.Should().Be(1654632000000L.FromDXFeed());
            candle[0].Time.Should().Be(1654632000000L.FromDXFeed());
            candle[1].Time.Should().Be(1654617600000L.FromDXFeed());
            candle[2].Time.Should().Be(1654603200000L.FromDXFeed());
            candle[3].Time.Should().Be(1654595013709L.FromDXFeed());
        }
    }
}
