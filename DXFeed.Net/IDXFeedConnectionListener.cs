using DXFeed.Net.DXFeedMessage;
using System;

namespace DXFeed.Net
{
    /// <summary>
    /// Connection 
    /// </summary>
    public interface IDXFeedConnectionListener
    {
        /// <summary>
        /// The method called when connection status is changed
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="state"></param>
        void OnStatusChanged(IDXFeedConnection connection, DXFeedConnectionState state);

        /// <summary>
        /// The method called when exception raised in the exception
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="status"></param>
        void OnException(IDXFeedConnection connection, Exception exception);

        /// <summary>
        /// Method called when the quote is received
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="quote"></param>
        void OnQuoteReceived(IDXFeedConnection connection, DXFeedResponseQuote quote);
    }
}
