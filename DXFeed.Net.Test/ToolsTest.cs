using FluentAssertions;
using System;
using Xunit;
using DXFeed.Net.Platform;
using DXFeed.Net.DXFeedMessage;

namespace DXFeed.Net.Test
{
    public class ToolsTest
    {
        [Fact]
        public void TestWait_Success()
        {
            bool rc = true;
            ((Action)(() => Tools.Wait(() => rc, 100)))
                .ExecutionTime().Should().BeLessThan(TimeSpan.FromSeconds(0.015));
        }

        [Fact]
        public void TestWait_Fail()
        {
            bool rc = false;
            ((Action)(() => Tools.Wait(() => rc, 100)))
                .ExecutionTime().Should().BeGreaterThan(TimeSpan.FromSeconds(0.095));
        }

        [Fact]
        public void DXTimeTest()
        {
            (new DateTime(2022, 7, 11, 0, 0, 0, DateTimeKind.Utc))
                .ToDXFeed()
                .Should().Be(1657497600000);

            1657843200000L.FromDXFeed()
                .Should().Be(new DateTime(2022, 7, 15, 0, 0, 0, DateTimeKind.Utc));
        }

        [Theory]
        [InlineData("AAPL{=d}", "AAPL", "d")]
        [InlineData("AAPL{=4h}", "AAPL", "4h")]
        [InlineData("EUR/USD{=d}", "EUR/USD", "d")]
        [InlineData("/CL{=d}", "/CL", "d")]
        [InlineData(".AAPL120616P255{=d}", ".AAPL120616P255", "d")]
        [InlineData("BAC/WS/A{=d}", "BAC/WS/A", "d")]
        [InlineData("IBM&M{=d}", "IBM&M", "d")]
        [InlineData("XRP/USD:CXDXF{=d}", "XRP/USD:CXDXF", "d")]
        public void CandleSymbolParse_Ok(string candle, string symbol, string period)
        {
            DXFeedCandleSymbol.TryParse(candle, out var candleSymbol).Should().BeTrue();
            candleSymbol.Symbol.Should().Be(symbol);
            candleSymbol.AggregationPeriod.Should().Be(period);

            candleSymbol.ToString().Should().Be(candle);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("AAPL")]
        [InlineData("AAPL{d}")]
        [InlineData("AA PL{=d}")]
        [InlineData("AAPL{=d")]
        public void CandleSymbolParse_Fail(string candle)
        {
            DXFeedCandleSymbol.TryParse(candle, out var _).Should().BeFalse();
        }
    }
}


