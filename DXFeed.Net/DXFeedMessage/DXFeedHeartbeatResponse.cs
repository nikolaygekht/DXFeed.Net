using DXFeed.Net.Message;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Response to a heartbeat
    /// </summary>
    public class DXFeedHeartbeatResponse : DXFeedResponse
    {
        /// <summary>
        /// If heartbeat successful
        /// </summary>
        public bool Successful { get; }

        /// <summary>
        /// The identifier of the client
        /// </summary>
        public string? ClientId { get; }

        /// <summary>
        /// The flag indicating whether the heartbeat response has a client id
        /// </summary>
        public bool HasClientId => !string.IsNullOrEmpty(ClientId);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message"></param>
        public DXFeedHeartbeatResponse(IMessageElementObject message) : base(DXFeedResponseType.Heartbeat)
        {
            if (message.HasProperty("clientId") && message["clientId"].AsString(out var clientId))
                ClientId = clientId;
            if (message.HasProperty("successful") && message["successful"].AsBoolean(out var successful))
                Successful = successful;
        }
    }
}
