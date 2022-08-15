using DXFeed.Net.Message;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Parser for responses
    /// </summary>
    internal static class DXFeedResponseParser
    {
        /// <summary>
        /// Parses the response
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static IDXFeedResponse? Parse(IMessageElementObject message)
        {
            if (message == null ||
                !message.HasProperty("channel") ||
                message["channel"].ElementType != MessageElementType.String)
                return null;

            string? channel = message["channel"].As<IMessageElementString>().Value;

            if (string.IsNullOrEmpty(channel))
                return null;

            switch (channel)
            {
                case "/meta/handshake":
                    return ParseHandshakeResponse(message);
                case "/meta/connect":
                    return ParseConnectResponse(message);
                default:
                    return new DXFeedUnknownResponse(message);
            }
        }

        private static IDXFeedResponse ParseConnectResponse(IMessageElementObject message)
            => new DXFeedResponseHeartbeat(message);

        private static IDXFeedResponse ParseHandshakeResponse(IMessageElementObject message)
            => new DXFeedResponseAuthorize(message);
    }
}
