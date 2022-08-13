namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// The advice returned during the authorization
    /// </summary>
    public class DXFeedAdvice
    {
        /// <summary>
        /// ???
        /// </summary>
        public int? Interval { get; }
        /// <summary>
        /// Timeout between heartbeats
        /// </summary>
        public int? Timeout { get; }
        /// <summary>
        /// ???
        /// </summary>
        public string? Reconnect { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="timeout"></param>
        /// <param name="reconnect"></param>
        public DXFeedAdvice(int? interval, int? timeout, string? reconnect)
        {
            Interval = interval;
            Timeout = timeout;
            Reconnect = reconnect;
        }
    }
}
