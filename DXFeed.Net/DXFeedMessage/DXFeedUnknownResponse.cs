using DXFeed.Net.Message;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Unknown response
    /// </summary>
    public class DXFeedUnknownResponse : DXFeedResponse
    {
        /// <summary>
        /// The original message
        /// </summary>
        public IMessageElementObject Message { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        public DXFeedUnknownResponse(IMessageElementObject message) : base(DXFeedResponseType.Unknown)
        {
            Message = message;
        }
    }
}
