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
    }
}
