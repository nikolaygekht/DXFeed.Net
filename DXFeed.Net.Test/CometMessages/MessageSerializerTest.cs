using DXFeed.Net.Message;
using DXFeed.Net.Platform;
using FluentAssertions;
using System;
using System.Threading.Channels;
using Xunit;

namespace DXFeed.Net.Test.CometMessages
{
    public class MessageSerializerTest
    {
        [Theory]
        [InlineData(typeof(MessageElementNull), null, "null")]
        [InlineData(typeof(MessageElementArray), null, "[]")]
        [InlineData(typeof(MessageElementObject), null, "{}")]
        [InlineData(typeof(MessageElementInteger), 123, "123")]
        [InlineData(typeof(MessageElementLong), 123, "123")]
        [InlineData(typeof(MessageElementDouble), 1.23, "1.23")]
        [InlineData(typeof(MessageElementBoolean), true, "true")]
        [InlineData(typeof(MessageElementBoolean), false, "false")]
        [InlineData(typeof(MessageElementString), "abc", "\"abc\"")]
        [InlineData(typeof(MessageElementString), "a\nc", "\"a\\nc\"")]
        public void Serialization_PrimitiveType(Type elementType, object value, string expected)
        {
            var message = MessageTestTools.CreateMessageElement(elementType, value != null ? 1 : 0, value);
            message.SerializeToString().Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(IMessageElementNull), null, "null")]
        [InlineData(typeof(IMessageElementArray), null, "[]")]
        [InlineData(typeof(IMessageElementObject), null, "{}")]
        [InlineData(typeof(IMessageElementInteger), 123, "123")]
        [InlineData(typeof(IMessageElementInteger), -123, "-123")]
        [InlineData(typeof(IMessageElementLong), 12345678901234L, "12345678901234")]
        [InlineData(typeof(IMessageElementDouble), 1.23, "1.23")]
        [InlineData(typeof(IMessageElementDouble), 1e5, "1e5")]
        [InlineData(typeof(IMessageElementBoolean), true, "true")]
        [InlineData(typeof(IMessageElementBoolean), false, "false")]
        [InlineData(typeof(IMessageElementString), "abc", "\"abc\"")]
        [InlineData(typeof(IMessageElementString), "a\nc", "\"a\\nc\"")]
        public void Deserialization_PrimitiveType(Type elementType, object value, string json)
        {
            var message = json.DeserializeToMessage();
            message.Should().BeAssignableTo(elementType);
            if (value != null)
                message.GetType().GetProperty("Value").GetValue(message, null).Should().Be(value);
        }

        [Fact]
        public void Deserialization_Array()
        {
            var message = "[1, \"abcd\", { \"a\" : \"b\" }, null ]".DeserializeToMessage();

            message.ElementType.Should().Be(MessageElementType.Array);
            var array = message.As<IMessageElementArray>();
            array.Length.Should().Be(4);

            array[0].ElementType.Should().Be(MessageElementType.Integer);
            array[1].ElementType.Should().Be(MessageElementType.String);
            array[2].ElementType.Should().Be(MessageElementType.Object);
            array[3].ElementType.Should().Be(MessageElementType.Null);
        }

        [Fact]
        public void Deserialization_Object()
        {
            var message = "{\"minimumVersion\":\"1.0\",\"clientId\":\"13farleu4sxZ3b1guicVNd85b44\",\"supportedConnectionTypes\":[\"websocket\"],\"advice\":{\"interval\":0,\"timeout\":30000,\"reconnect\":\"retry\"},\"channel\":\"/meta/handshake\",\"version\":\"1.0\",\"successful\":true}".DeserializeToMessage();

            message.ElementType.Should().Be(MessageElementType.Object);
            var @object = message.As<IMessageElementObject>();

            @object.HasProperty("minimumVersion").Should().BeTrue();

            @object["minimumVersion"]
                .Should().BeAssignableTo<IMessageElementString>()
                .Which.Value.Should().Be("1.0");
            
            @object.HasProperty("clientId").Should().BeTrue();
            @object["clientId"]
                .Should().BeAssignableTo<IMessageElementString>()
                .Which.Value.Should().Be("13farleu4sxZ3b1guicVNd85b44");

            @object.HasProperty("supportedConnectionTypes").Should().BeTrue();
            @object["supportedConnectionTypes"]
                .Should().BeAssignableTo<IMessageElementArray>()
                .Which.Should().HaveCount(1)
                               .And.Contain(x => x.ElementType == MessageElementType.String && x.As<IMessageElementString>().Value == "websocket");

            @object.HasProperty("advice").Should().BeTrue();
            @object["advice"]
                .Should().BeAssignableTo<IMessageElementObject>();

            var advice = @object["advice"].As<IMessageElementObject>();
            advice.HasProperty("timeout").Should().BeTrue();
            advice["timeout"]
                .Should().BeAssignableTo<IMessageElementInteger>()
                .Which.Value.Should().Be(30000);

            @object.HasProperty("channel").Should().BeTrue();
            @object["channel"]
                .Should().BeAssignableTo<IMessageElementString>()
                .Which.Value.Should().Be("/meta/handshake");

            @object.HasProperty("version").Should().BeTrue();
            @object["version"]
                .Should().BeAssignableTo<IMessageElementString>()
                .Which.Value.Should().Be("1.0");

            @object.HasProperty("successful").Should().BeTrue();
            @object["successful"]
                .Should().BeAssignableTo<IMessageElementBoolean>()
                .Which.Value.Should().Be(true);
        }
    }
}

