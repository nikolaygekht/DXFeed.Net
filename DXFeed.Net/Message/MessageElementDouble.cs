namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is an real value
    /// </summary>
    public class MessageElementDouble : MessageElement, IMessageElementDouble
    {
        /// <summary>
        /// The value
        /// </summary>
        public double Value { get; set; }

        public MessageElementDouble(double value) : base(MessageElementType.Double)
        {
            Value = value;
        }
    }
}
