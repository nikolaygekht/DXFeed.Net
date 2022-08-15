using DXFeed.Net.Message;
using FluentAssertions;
using System;
using System.Reflection;

namespace DXFeed.Net.Test.CometMessages
{
    internal static class MessageTestTools
    {
        public static IMessageElement CreateMessageElement(Type elementType, int constructorArgsCount, object constructorParameterValue)
        {
            Type[] constructorArgTypes;
            object[] constructorArgs;
            if (constructorArgsCount == 0)
            {
                constructorArgTypes = Array.Empty<Type>();
                constructorArgs = Array.Empty<object>();
            }
            else
            {
                constructorParameterValue.Should().NotBeNull("the type constructor has a parameter");
                constructorArgTypes = new Type[] { constructorParameterValue.GetType() };
                constructorArgs = new object[] { constructorParameterValue };
            }

            ConstructorInfo constructor = elementType.GetConstructor(constructorArgTypes);
            constructor.Should().NotBeNull();

            var element = constructor.Invoke(constructorArgs);
            element.Should().BeAssignableTo<IMessageElement>();
            return (IMessageElement)element;
        }
    }
}

