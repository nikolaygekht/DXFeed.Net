using DXFeed.Net.DXFeedMessage;
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
        ICommunicator Communicator { get; }

        /// <summary>
        /// Subscribe for quotes
        /// </summary>
        /// <param name="symbols"></param>
        void SubscribeForQuotes(string[] symbols);

        /// <summary>
        /// Unsubscribe from quotes
        /// </summary>
        void UnsubscribeFromQuotes(string[] symbols);

        /// <summary>
        /// Subscribe for candles
        /// </summary>
        /// <param name="candles"></param>
        void SubscribeForCandles(DXFeedCandleRequest[] candles);

        /// <summary>
        /// Unsubscribe from candles
        /// </summary>
        void UnsubscribeFromCandles(DXFeedCandleRequest[] candles);
    }
}
