using DXFeed.Net.Message;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Response to the authorization
    /// </summary>
    public class DXFeedResponseAuthorize : DXFeedResponse
    {
        public DXFeedAdvice? Advice { get; }

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
        public DXFeedResponseAuthorize(IMessageElementObject message) : base(DXFeedResponseType.Authorize)
        {
            if (message.HasProperty("clientId") && message["clientId"].AsString(out var clientId))
                ClientId = clientId;
            if (message.HasProperty("successful") && message["successful"].AsBoolean(out var successful))
                Successful = successful;
            if (message.HasProperty<IMessageElementObject>("advice", out var advice))
            {
                Advice = ParseAdvice(advice);
            }
        }

        private static DXFeedAdvice? ParseAdvice(IMessageElementObject? advice)
        {
            if (advice == null)
                return null;
            int? interval = null, timeout = null;
            string? reconnect = null;

            if (advice.HasProperty("interval") && advice["interval"].AsInteger(out var intervalValue))
                interval = intervalValue;
            
            if (advice.HasProperty("timeout") && advice["timeout"].AsInteger(out var timeoutValue))
                timeout = timeoutValue;

            if (advice.HasProperty("reconnect") && advice["reconnect"].AsString(out var reconnectValue))
                reconnect = reconnectValue;

            return new DXFeedAdvice(interval, timeout, reconnect);
        }
    }
}
