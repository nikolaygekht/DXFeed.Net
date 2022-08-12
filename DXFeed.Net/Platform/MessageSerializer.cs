using DXFeed.Net.Message;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

//NOTE: I know that serialization via nodes and deserialization via elements (different levels of System.Text.Json API)
//looks weird. Reasoning:
//  - JsonElement is used to deserialization because it is possible to identify
//    the value types and enumerate properies (hard to do in JsonNode)
//  - JsonNode is used to serialization because it is possible to construct
//    objects (hard to do in JsonElement) 

namespace DXFeed.Net.Platform
{
    /// <summary>
    /// Serializer and deserializer for message into json
    /// </summary>
    public static class MessageSerializer
    {
        /// <summary>
        /// Serializes the message to a json string
        /// </summary>
        /// <param name="messageElement"></param>
        /// <returns></returns>
        public static string SerializeToString(this IMessageElement messageElement)
            => JsonSerializer.Serialize(SerializeToJson(messageElement));

        /// <summary>
        /// Serializes the message to a json element
        /// </summary>
        public static JsonNode? SerializeToJson(this IMessageElement messageElement)
        {
            return messageElement.ElementType switch
            {
                MessageElementType.Object => SerializeToJson(messageElement.As<IMessageElementObject>()),
                MessageElementType.Array => SerializeToJson(messageElement.As<IMessageElementArray>()),
                MessageElementType.Null => null,
                MessageElementType.Boolean => JsonValue.Create(messageElement.As<IMessageElementBoolean>().Value),
                MessageElementType.Integer => JsonValue.Create(messageElement.As<IMessageElementInteger>().Value),
                MessageElementType.Long => JsonValue.Create(messageElement.As<IMessageElementLong>().Value),
                MessageElementType.Double => JsonValue.Create(messageElement.As<IMessageElementDouble>().Value),
                MessageElementType.String => JsonValue.Create(messageElement.As<IMessageElementString>().Value),
                _ => throw new ArgumentException($"Argument has unsupported type {messageElement.GetType()}", nameof(messageElement)),
            }; 
        }

        private static JsonObject SerializeToJson(this IMessageElementObject @object)
        {
            var jsonObject = new JsonObject();
            foreach (var property in @object)
            {
                var value = SerializeToJson(property.Value);
                jsonObject.Add(property.Name, value);
            }
            return jsonObject;
        }


        private static JsonArray SerializeToJson(this IMessageElementArray array)
        {
            var jsonArray = new JsonArray();
            for (int i = 0; i < array.Length; i++)
                jsonArray.Add(SerializeToJson(array[i]));
            return jsonArray;
        }

        /// <summary>
        /// Deserializes a string to message
        /// </summary>
        /// <param name="jsonText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IMessageElement DeserializeToMessage(this string jsonText)
        {
            if (jsonText == null)
                throw new ArgumentNullException(nameof(jsonText));
            var element = JsonSerializer.Deserialize<JsonElement>(jsonText);

            return element.DeserializeToMessage();
        }

        /// <summary>
        /// Deserializes a json element to message
        /// </summary>
        /// <param name="jsonElement"></param>
        /// <returns></returns>
        public static IMessageElement DeserializeToMessage(this JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Object)
                return DeserializeObjectToMessage(jsonElement);
            else if (jsonElement.ValueKind == JsonValueKind.Array)
                return DeserializeArrayToMessage(jsonElement);
            else if (jsonElement.ValueKind == JsonValueKind.True)
                return new MessageElementBoolean(true);
            else if (jsonElement.ValueKind == JsonValueKind.False)
                return new MessageElementBoolean(false);
            else if (jsonElement.ValueKind == JsonValueKind.String)
                return new MessageElementString(jsonElement.GetString());
            else if (jsonElement.ValueKind == JsonValueKind.Null)
                return new MessageElementNull();
            else if (jsonElement.ValueKind == JsonValueKind.Number)
                return DeserializeNumberValueToMessage(jsonElement.GetRawText());
            
            throw new ArgumentException($"Unsupported json node type {jsonElement.ValueKind}", nameof(jsonElement));
        }

        private static IMessageElementObject DeserializeObjectToMessage(this JsonElement jsonArray)
        {
            var @object = new MessageElementObject();
            using var @enumerator = jsonArray.EnumerateObject();
            
            @enumerator.Reset();
            while (@enumerator.MoveNext())
            {
                var property = enumerator.Current;
                @object[property.Name] = property.Value.DeserializeToMessage();
            }

            return @object;
        }

        private static IMessageElementArray DeserializeArrayToMessage(this JsonElement jsonArray)
        {
            var array = new MessageElementArray();
            for (int i = 0; i < jsonArray.GetArrayLength(); i++)
                array.Add(DeserializeToMessage(jsonArray[i]));
            return array;
        }

        private static IMessageElement DeserializeNumberValueToMessage(string v)
        {
            if (v.Contains('.') || v.Contains('e') || v.Contains('E'))
                return new MessageElementDouble(double.Parse(v, CultureInfo.InvariantCulture));

            long x = long.Parse(v, CultureInfo.InvariantCulture);
            if (x > int.MaxValue || x < int.MinValue)
                return new MessageElementLong(x);
            else
                return new MessageElementInteger((int)x);
        }
    }
}
