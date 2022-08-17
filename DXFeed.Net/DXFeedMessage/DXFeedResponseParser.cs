using DXFeed.Net.Message;
using System.Collections.Generic;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Parser for responses
    /// </summary>
    internal static class DXFeedResponseParser
    {
        private static List<string>? mQuoteFields = null;
        private static List<string>? mCandleFields = null;

        /// <summary>
        /// Converts an array into a list of strings
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        private static List<string> ToList(IMessageElementArray array)
        {
            var l = new List<string>();
            for (int i = 0; i < array.Length; i++)
                if (array[i].AsString(out var s))
                    l.Add(s);
            return l;
        }

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
                case "/service/data":
                case "/service/timeSeriesData":
                    {
                        var x = ParseData(message);
                        if (x == null)
                            return new DXFeedUnknownResponse(message);
                        return x;
                    }
                default:
                    return new DXFeedUnknownResponse(message);
            }
        }

        private static IDXFeedResponse ParseConnectResponse(IMessageElementObject message)
            => new DXFeedResponseHeartbeat(message);

        private static IDXFeedResponse ParseHandshakeResponse(IMessageElementObject message)
            => new DXFeedResponseAuthorize(message);

        private static IDXFeedResponse? ParseData(IMessageElementObject message)
        {
            if (!message.HasProperty("data") || message["data"].ElementType != MessageElementType.Array)
                return null;
            
            var data = message["data"].As<IMessageElementArray>();
            if (data.Length != 2)        //data is [metadata, data]
                return null;

            string type;
            
            //handle metadata
            var data0 = data[0];
            if (data0.ElementType == MessageElementType.Array)
            {
                //it is first message and metadata has format [messagetype, [fields]]
                
                var meta = data0.As<IMessageElementArray>();
                if (meta.Length != 2)
                    return null;
                
                if (!meta[0].AsString(out type))
                    return null;

                var fields = meta[1];
                if (fields.ElementType != MessageElementType.Array)
                    return null;
                
                var l = ToList(fields.As<IMessageElementArray>());
                if (l.Count == 0)
                    return null;

                if (type == "Quote")
                    mQuoteFields = l;
                else if (type == "Candle")
                    mCandleFields = l;

            }
            else if (!data0.AsString(out type))
                return null;

            if (type == "Quote")
            {
                var meta = mQuoteFields;
                if (meta == null)
                    return null;

                var data1 = data[1];
                if (data1.ElementType != MessageElementType.Array)
                    return null;
                var arr = data1.As<IMessageElementArray>();
                if (arr.Length < meta.Count)
                    return null;
                return new DXFeedResponseQuote(meta, arr);
            }
            if (type == "Candle")
            {
                var meta = mCandleFields;
                if (meta == null)
                    return null;

                var data1 = data[1];
                if (data1.ElementType != MessageElementType.Array)
                    return null;
                var arr = data1.As<IMessageElementArray>();
                if (arr.Length < meta.Count)
                    return null;
                return new DXFeedResponseCandle(meta, arr);
            }
            return null;
        }
    }
}
