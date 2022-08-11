namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is a string
    /// </summary>
    public class MessageElementString : MessageElement, IMessageElementString
    {
        /// <summary>
        /// The value
        /// </summary>
        public string? Value { get; set; }

        public MessageElementString() : base(MessageElementType.String) { }

        public MessageElementString(string? value) : this() { Value = value; }
    }
}
