namespace DXFeed.Net.Message
{
    /// <summary>
    /// One property of a message object element
    /// </summary>
    public interface IMessageElementObjectProperty
    {
        /// <summary>
        /// The name of the property
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The value
        /// </summary>
        IMessageElement Value { get; set; }
    }
}
