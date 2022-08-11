namespace DXFeed.Net.Message
{
    /// <summary>
    /// An element of a message
    /// </summary>
    public abstract class MessageElement : IMessageElement
    {
        /// <summary>
        /// The type of the element
        /// </summary>
        public MessageElementType ElementType { get; }

        protected MessageElement(MessageElementType type)
        {
            ElementType = type;
        }
    }
}
