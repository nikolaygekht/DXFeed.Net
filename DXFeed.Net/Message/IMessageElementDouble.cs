namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is an real value
    /// </summary>
    public interface IMessageElementDouble : IMessageElement
    {
        /// <summary>
        /// The value
        /// </summary>
        double Value { get; set; }
    }
}
