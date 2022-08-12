namespace DXFeed.Net.Message
{
    /// <summary>
    /// One property of a message object element
    /// </summary>
    public class MessageElementObjectProperty : IMessageElementObjectProperty
    {
        /// <summary>
        /// The name of the property
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The value
        /// </summary>
        public IMessageElement Value { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public MessageElementObjectProperty(string name, IMessageElement value)
        {
            Name = name;
            Value = value;
        }
    }
}
