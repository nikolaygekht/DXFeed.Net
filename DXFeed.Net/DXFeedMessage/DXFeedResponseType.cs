namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Response identifiers
    /// </summary>
    public enum DXFeedResponseType
    {
        /// <summary>
        /// Unknown Response
        /// </summary>
        Unknown,

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
