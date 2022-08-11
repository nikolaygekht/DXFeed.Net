using System;
using System.Collections.Generic;
using System.Text;

namespace DXFeed.Net.Platform
{
    /// <summary>
    /// The interface to the transport
    /// </summary>
    public interface ITransport : IDisposable
    {

        /// <summary>
        /// The status of the transport
        /// </summary>
        TransportState State { get; }

        /// <summary>
        /// The encoding for the messages
        /// </summary>
        Encoding Encoding { get; set; }

        /// <summary>
        /// Connect the transport to a endpoint
        /// </summary>
        /// <param name="url"></param>
        void Connect(string url);

        /// <summary>
        /// Close the connection
        /// </summary>
        void Close();

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="message"></param>
        void Send(string message);

        /// <summary>
        /// Receive a message
        /// </summary>
        /// <returns></returns>
        string? Receive();
    }
}
