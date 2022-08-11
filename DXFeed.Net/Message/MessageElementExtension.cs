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
    }
}
