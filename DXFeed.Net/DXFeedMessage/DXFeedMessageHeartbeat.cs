using DXFeed.Net.Message;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Heartbeat message
    /// </summary>
    public class DXFeedMessageHeartbeat : DXFeedMessage
    {
        private readonly string mClientId; 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clientId"></param>
        public DXFeedMessageHeartbeat(string clientId) : base("/meta/connect", DXFeedMessageType.Heartbeat)
        {
            mClientId = clientId;
        }

        protected override void ConfigureMessage(IMessageElementObject message)
        {
            message["clientId"] = new MessageElementString(mClientId);
        }
    }
}
