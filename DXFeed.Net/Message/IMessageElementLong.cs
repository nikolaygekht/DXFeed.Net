namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is an 64-bit integer
    /// </summary>
    public interface IMessageElementLong : IMessageElement
    {
        /// <summary>
        /// The value
        /// </summary>
        long Value { get; set; }
    }
}
