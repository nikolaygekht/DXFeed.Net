using DXFeed.Net.Message;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Authorization message
    /// </summary>
    public class DXFeedMessageAuthorize : DXFeedMessage
    {
        private readonly string mToken;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token"></param>
        public DXFeedMessageAuthorize(string token) : base("/meta/handshake", DXFeedMessageType.Authorize)
        {
            mToken = token;
        }

        protected override void ConfigureMessage(IMessageElementObject message)
        {
            message["ext"] = new MessageElementObject()
            {
                { "com.devexperts.auth.AuthToken", new MessageElementString(mToken) }
            };
        }
    }
}
