namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is an 32-bit integer
    /// </summary>
    public interface IMessageElementInteger : IMessageElement
    {
        /// <summary>
        /// The value
        /// </summary>
        int Value { get; set; }
    }
}
