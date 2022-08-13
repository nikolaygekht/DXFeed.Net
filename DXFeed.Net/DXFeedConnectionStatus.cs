namespace DXFeed.Net
{
    /// <summary>
    /// Connection status
    /// </summary>
    public enum DXFeedConnectionState
    {
        /// <summary>
        /// Connected but need to be authorized
        /// </summary>
        ReadyToConnect,
        /// <summary>
        /// Connected but need to be authorized
        /// </summary>
        Connecting,
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
