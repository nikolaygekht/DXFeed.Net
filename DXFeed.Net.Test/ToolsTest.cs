using FluentAssertions;
using System;
using Xunit;
using DXFeed.Net.Platform;

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
    }
}
