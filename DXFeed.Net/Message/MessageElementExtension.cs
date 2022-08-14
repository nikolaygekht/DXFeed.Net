using System.Globalization;

namespace DXFeed.Net.Message
{
    /// <summary>
    /// The extension for the message element classes
    /// </summary>
    public static class MessageElementExtension
    {
        /// <summary>
        /// Get a message element as a specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static T As<T>(this IMessageElement element) where T : IMessageElement => (T)element;

        /// <summary>
        /// Checks whether the object has a property of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool HasProperty<T>(this IMessageElementObject @object, string name, out T? value) where T : class, IMessageElement
        {
            value = default;
            if (!@object.HasProperty(name))
                return false;
            var v = @object[name];
            if (v is T t)
            {
                value = t;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets an element value as a string
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool AsString(this IMessageElement element, out string value)
        {
            value = "";
            switch (element)
            {
                case IMessageElementInteger intValue:
                    value = intValue.Value.ToString(CultureInfo.InvariantCulture);
                    return true;
                case IMessageElementLong longValue:
                    value = longValue.Value.ToString(CultureInfo.InvariantCulture);
                    return true;
                case IMessageElementDouble doubleValue:
                    value = doubleValue.Value.ToString(CultureInfo.InvariantCulture);
                    return true;
                case IMessageElementBoolean booleanValue:
                    value = booleanValue.Value ? "true" : "false";
                    return true;
                case IMessageElementString stringValue:
                    value = stringValue.Value ?? "";
                    return true;
                case IMessageElementObject _:
                case IMessageElementArray _:
                case IMessageElementNull _:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets an element value as a boolean
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool AsBoolean(this IMessageElement element, out bool value)
        {
            value = false;
            switch (element)
            {
                case IMessageElementInteger intValue:
                    return false;
                case IMessageElementLong longValue:
                    return false;
                case IMessageElementDouble doubleValue:
                    return false;
                case IMessageElementBoolean booleanValue:
                    value = booleanValue.Value;
                    return true;
                case IMessageElementString stringValue:
                    if (stringValue.Value == "true")
                    {
                        value = true;
                        return true;
                    }
                    else if (stringValue.Value == "false")
                    {
                        value = false;
                        return true;
                    }
                    return false;
                case IMessageElementObject _:
                case IMessageElementArray _:
                case IMessageElementNull _:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets an element value as a string
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool AsInteger(this IMessageElement element, out int value)
        {
            value = 0;
            switch (element)
            {
                case IMessageElementInteger intValue:
                    value = intValue.Value;
                    return true;
                case IMessageElementLong longValue:
                    value = (int)longValue.Value;
                    return true;
                case IMessageElementDouble doubleValue:
                    value = (int)doubleValue.Value;
                    return true;
                case IMessageElementBoolean booleanValue:
                    return false;
                case IMessageElementString stringValue:
                    if (int.TryParse(stringValue.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
                    {
                        value = v;
                        return true;
                    }
                    return false;
                case IMessageElementObject _:
                case IMessageElementArray _:
                case IMessageElementNull _:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets an element value as a string
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool AsLong(this IMessageElement element, out long value)
        {
            value = 0;
            switch (element)
            {
                case IMessageElementInteger intValue:
                    value = intValue.Value;
                    return true;
                case IMessageElementLong longValue:
                    value = longValue.Value;
                    return true;
                case IMessageElementDouble doubleValue:
                    value = (long)doubleValue.Value;
                    return true;
                case IMessageElementBoolean booleanValue:
                    return false;
                case IMessageElementString stringValue:
                    if (long.TryParse(stringValue.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
                    {
                        value = v;
                        return true;
                    }
                    return false;
                case IMessageElementObject _:
                case IMessageElementArray _:
                case IMessageElementNull _:
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets an element value as a string
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static bool AsDouble(this IMessageElement element, out double value)
        {
            value = 0;
            switch (element)
            {
                case IMessageElementInteger intValue:
                    value = intValue.Value;
                    return true;
                case IMessageElementLong longValue:
                    value = longValue.Value;
                    return true;
                case IMessageElementDouble doubleValue:
                    value = doubleValue.Value;
                    return true;
                case IMessageElementBoolean booleanValue:
                    return false;
                case IMessageElementString stringValue:
                    if (double.TryParse(stringValue.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                    {
                        value = v;
                        return true;
                    }
                    return false;
                case IMessageElementObject _:
                case IMessageElementArray _:
                case IMessageElementNull _:
                default:
                    return false;
            }
        }
    }
}
