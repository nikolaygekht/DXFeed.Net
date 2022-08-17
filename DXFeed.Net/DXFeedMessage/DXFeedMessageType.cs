namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Message identifiers
    /// </summary>
    public enum DXFeedMessageType
    {
        /// <summary>
        /// Message to authorize via token
        /// </summary>
        Authorize,
        
        /// <summary>
        /// Heartbeat message
        /// </summary>
        Heartbeat,

        /// <summary>
        /// Subscribe for quotes message
        /// </summary>
        SubscribeForQuotes,

        /// <summary>
        /// Subscribe for candles message
        /// </summary>
        SubscribeForCandles,
    }
}
