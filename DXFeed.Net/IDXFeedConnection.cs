using DXFeed.Net.Platform;
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
        string? ClientId { get; }

        /// <summary>
        /// Current connection status.
        /// </summary>
        DXFeedConnectionState State { get; }

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

        /// <summary>
        /// Associated communicator
        /// </summary>
        public ICommunicator Communicator { get; }

        /// <summary>
        /// Subscribe for quotes
        /// </summary>
        /// <param name="symbols"></param>
        public void SubscribeForQuotes(string[] symbols);

        /// <summary>
        /// Unsubscribe from quotes
        /// </summary>
        public void UnsubscribeFromQuotes(string[] symbols);
    }
}
