using System;

namespace DXFeed.Net
{
    /// <summary>
    /// Interface to connect
    /// </summary>
    public interface IDXFeedConnection : IDisposable
    {
        /// <summary>
        /// Id of the current client session
        /// </summary>
        string ClientId { get; }

        /// <summary>
        /// Current connection status.
        /// </summary>
        DXFeedConnectionStatus Status { get; }

        /// <summary>
        /// Subscribe listener 
        /// </summary>
        /// <param name="listener"></param>
        void SubscribeListener(IDXFeedConnectionListener listener);

        /// <summary>
        /// Unsubscribe listener
        /// </summary>
        /// <param name="listener"></param>
        void UnsubscribeListener(IDXFeedConnectionListener listener);
    }
}
