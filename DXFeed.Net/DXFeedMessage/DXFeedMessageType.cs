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
    }
}
