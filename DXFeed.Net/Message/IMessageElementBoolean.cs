namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is a boolean (true or false) value
    /// </summary>
    public interface IMessageElementBoolean : IMessageElement
    {
        /// <summary>
        /// The value
        /// </summary>
        bool Value { get; set; }
    }
}
