namespace DXFeed.Net
{
    /// <summary>
    /// Connection status
    /// </summary>
    public enum DXFeedConnectionStatus
    {
        /// <summary>
        /// Connected but need to be authorized
        /// </summary>
        Connected,
        /// <summary>
        /// Connected and ready to work
        /// </summary>
        Ready,
        /// <summary>
        /// Disconnected.
        /// </summary>
        Disconnected,
    }
}
