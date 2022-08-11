namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is an 32-bit integer
    /// </summary>
    public class MessageElementInteger : MessageElement, IMessageElementInteger
    {
        /// <summary>
        /// The value
        /// </summary>
        public int Value { get; set; }

        public MessageElementInteger(int value) : base(MessageElementType.Integer)
        {
            Value = value;
        }
    }
}
