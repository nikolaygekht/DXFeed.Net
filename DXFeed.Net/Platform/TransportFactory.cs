namespace DXFeed.Net.Platform
{
    /// <summary>
    /// The transport factory
    /// </summary>
    public static class TransportFactory
    {
        /// <summary>
        /// Create a websocket transport
        /// </summary>
        /// <returns></returns>
        public static ITransport CreateWebsocketTransport() => new WebSocketTransport();
    }
}
