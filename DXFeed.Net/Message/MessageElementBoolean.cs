namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is a boolean (true or false) value
    /// </summary>
    public class MessageElementBoolean : MessageElement, IMessageElementBoolean
    {
        /// <summary>
        /// The value
        /// </summary>
        public bool Value { get; set; }

        public MessageElementBoolean(bool value) : base(MessageElementType.Boolean)
        {
            Value = value;
        }
    }
}
