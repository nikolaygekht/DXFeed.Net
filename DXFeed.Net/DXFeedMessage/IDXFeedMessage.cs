using DXFeed.Net.Message;
using DXFeed.Net.Platform;
using System;
using System.Collections.Generic;
using System.Text;

namespace DXFeed.Net.DXFeedMessage
{

    /// <summary>
    /// Generic interface to IDXFeedMessage
    /// </summary>
    public interface IDXFeedMessage
    {
        /// <summary>
        /// Returns message type
        /// </summary>
        DXFeedMessageType MessageType { get; }

        /// <summary>
        /// Gets message object
        /// </summary>
        /// <returns></returns>
        IMessageElementObject ToMessage();
    }
}
