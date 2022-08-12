using DXFeed.Net.Message;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DXFeed.Net.Test.Model
{

    public class MessageTest
    {
        [Theory]
        [InlineData(MessageElementType.Object, typeof(MessageElementObject), 0)]
        [InlineData(MessageElementType.Array, typeof(MessageElementArray), 0)]
        [InlineData(MessageElementType.Null, typeof(MessageElementNull), 0)]
        [InlineData(MessageElementType.String, typeof(MessageElementString), 1, "123")]
        [InlineData(MessageElementType.Integer, typeof(MessageElementInteger), 1, 123)]
        [InlineData(MessageElementType.Long, typeof(MessageElementLong), 1, 123L)]
        [InlineData(MessageElementType.Boolean, typeof(MessageElementBoolean), 1, true)]
        [InlineData(MessageElementType.Double, typeof(MessageElementDouble), 1, 1.23)]
        public void Type(MessageElementType expectedType, Type elementType, int constructorArgsCount, object constructorParameterValue = null)
        {
            var element = MessageTestTools.CreateMessageElement(elementType, constructorArgsCount, constructorParameterValue);
            element.ElementType.Should().Be(expectedType);

        }

        [Theory]
        [InlineData(typeof(MessageElementString), 1, "123")]
        [InlineData(typeof(MessageElementInteger), 1, 123)]
        [InlineData(typeof(MessageElementLong), 1, 123L)]
        [InlineData(typeof(MessageElementBoolean), 1, true)]
        [InlineData(typeof(MessageElementDouble), 1, 1.23)]
        public void Value(Type elementType, int constructorArgsCount, object constructorParameterValue)
        {
            var element = MessageTestTools.CreateMessageElement(elementType, constructorArgsCount, constructorParameterValue);
            var valueProperty = elementType.GetProperty("Value");
            valueProperty.Should().NotBeNull();
            valueProperty.PropertyType.Should().Be(constructorParameterValue.GetType());
            valueProperty.GetValue(element, null).Should().Be(constructorParameterValue);
        }

        [Fact]
        public void PropertyConstructor()
        {
            var property = new MessageElementObjectProperty("thename", new MessageElementString("123"));
            property.Name.Should().Be("thename");
            property.Value.Should().NotBeNull();
            property.Value.ElementType.Should().Be(MessageElementType.String);
            property.Value.As<MessageElementString>().Value.Should().Be("123");
        }

        [Fact]
        public void Object_Empty()
        {
            var message = new MessageElementObject();

            message.HasProperty("name").Should().BeFalse();
            message.Should().BeEmpty();
            ((Action)(() => _ = message["name"])).Should().Throw<KeyNotFoundException>();
        }

        [Fact]
        public void Object_AssignProperty()
        {
            var message = new MessageElementObject();
            message["name"] = new MessageElementString("123");
            message.HasProperty("name").Should().BeTrue();
            ((Action)(() => _ = message["name"])).Should().NotThrow();

            message["name"].Should().NotBeNull();
            message["name"].Should()
                .BeOfType<MessageElementString>()
                .Which.Value.Should().Be("123");
            message.Should().HaveCount(1);
        }

        [Fact]
        public void Object_ReassignProperty()
        {
            var message = new MessageElementObject();
            message["name"] = new MessageElementInteger(123);

            message["name"] = new MessageElementString("123");

            message.HasProperty("name").Should().BeTrue();
            ((Action)(() => _ = message["name"])).Should().NotThrow();

            message["name"].Should().NotBeNull();
            message["name"].Should()
                .BeOfType<MessageElementString>()
                .Which.Value.Should().Be("123");
            message.Should().HaveCount(1);
        }

        [Fact]
        public void Object_AssignTwoProperties()
        {
            var message = new MessageElementObject();
            message["name1"] = new MessageElementInteger(123);
            message["name2"] = new MessageElementString("123");

            message.Should()
                .HaveCount(2)
                .And.Contain(x => x.Name == "name1" && x.Value is MessageElementInteger)
                .And.Contain(x => x.Name == "name2" && x.Value is MessageElementString);
        }

        [Fact]
        public void Array_Empty()
        {
            var array = new MessageElementArray();
            array.Length.Should().Be(0);
            array.Should().BeEmpty();
            ((Action)(() => _ = array[0])).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Array_IndexAccess()
        {
            var array = new MessageElementArray();
            var i1 = new MessageElementInteger(1);
            var i2 = new MessageElementInteger(2);

            array.Add(i1);
            array.Add(i2);

            ((Action)(() => _ = array[-1])).Should().Throw<ArgumentOutOfRangeException>();
            ((Action)(() => _ = array[0])).Should().NotThrow();
            ((Action)(() => _ = array[1])).Should().NotThrow();
            ((Action)(() => _ = array[2])).Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Array_IndexValues_Add()
        {
            var array = new MessageElementArray();
            var i1 = new MessageElementInteger(1);
            var i2 = new MessageElementInteger(2);

            array.Add(i1);
            array.Add(i2);

            array[0]
                .Should().BeSameAs(i1);

            array[1]
                .Should().BeSameAs(i2);
        }

        [Fact]
        public void Array_IndexValues_Assign()
        {
            var array = new MessageElementArray();
            var i1 = new MessageElementInteger(1);
            var i2 = new MessageElementInteger(2);
            var i3 = new MessageElementInteger(3);
            var i4 = new MessageElementInteger(3);

            array.Add(i1);
            array.Add(i2);
            array[0] = i3;
            array[1] = i4;

            array[0]
                .Should().BeSameAs(i3);

            array[1]
                .Should().BeSameAs(i4);
        }

        [Fact]
        public void Array_Enumeration()
        {
            var array = new MessageElementArray();
            var i1 = new MessageElementInteger(1);
            var i2 = new MessageElementInteger(2);

            array.Add(i1);
            array.Add(i2);

            array.Length.Should().Be(2);
            array.Should().HaveCount(2);
            array.Should().Contain(x => object.ReferenceEquals(x, i1));
            array.Should().Contain(x => object.ReferenceEquals(x, i2));
        }

    }
}

