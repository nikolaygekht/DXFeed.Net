namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is a string
    /// </summary>
    public interface IMessageElementString : IMessageElement
    {
        /// <summary>
        /// The value
        /// </summary>
        string? Value { get; set; }
    }
}
