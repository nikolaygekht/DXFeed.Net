namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is an 64-bit integer
    /// </summary>
    public class MessageElementLong : MessageElement, IMessageElementLong
    {
        /// <summary>
        /// The value
        /// </summary>
        public long Value { get; set; }

        public MessageElementLong(long value) : base(MessageElementType.Long)
        {
            Value = value;
        }
    }
}
