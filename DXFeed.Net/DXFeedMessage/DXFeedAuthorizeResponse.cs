using DXFeed.Net.Message;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Response to the authorization
    /// </summary>
    public class DXFeedAuthorizeResponse : DXFeedResponse
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
        public DXFeedAuthorizeResponse(IMessageElementObject message) : base(DXFeedResponseType.Heartbeat)
        {
            if (message.HasProperty<IMessageElementString>("clientId", out var clientId))
                ClientId = clientId?.Value;
            if (message.HasProperty<IMessageElementString>("successful", out var successfulString))
                Successful = successfulString?.Value == "true";
            if (message.HasProperty<IMessageElementBoolean>("successful", out var succesfulBoolean))
                Successful = succesfulBoolean?.Value ?? false;
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

            if (advice.HasProperty<IMessageElementInteger>("interval", out var intervalValue))
                interval = intervalValue?.Value;
            if (advice.HasProperty<IMessageElementInteger>("timeout", out var timeoutValue))
                timeout = timeoutValue?.Value;
            if (advice.HasProperty<IMessageElementString>("reconnect", out var reconnectValue))
                reconnect = reconnectValue?.Value;
            return new DXFeedAdvice(interval, timeout, reconnect);
        }
    }
}
