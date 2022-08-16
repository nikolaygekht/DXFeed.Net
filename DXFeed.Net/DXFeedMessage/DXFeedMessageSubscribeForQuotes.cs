using DXFeed.Net.Message;
using System;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Subscribe for quotes message
    /// </summary>
    public class DXFeedMessageSubscribeForQuotes : DXFeedMessage
    {
        private readonly string[] mSymbols;
        private readonly string mClientId;
        private readonly DXFeedSubscribeMode mMode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="symbol"></param>
        /// <param name="clientId"></param>
        public DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode mode, string symbol, string clientId)
            : this(mode, new string[] { symbol }, clientId)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token"></param>
        public DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode mode, string[] symbols, string clientId) : base("/service/sub", DXFeedMessageType.SubscribeForQuotes)
        {
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("Client id is empty", nameof(clientId));

            if (symbols == null)
                throw new ArgumentNullException(nameof(symbols));

            if (symbols.Length == 0)
                throw new ArgumentException("The symbol list is empty", nameof(symbols));

            for (int i = 0; i < symbols.Length; i++)
                if (string.IsNullOrEmpty(symbols[i]))
                    throw new ArgumentException($"The symbol at position {i} is empty", nameof(symbols));

            mSymbols = new string[symbols.Length];
            symbols.CopyTo(mSymbols, 0);
            
            mClientId = clientId;
            mMode = mode;
        }

        protected override void ConfigureMessage(IMessageElementObject message)
        {
            var symbols = new MessageElementArray();
            for (int i = 0; i < mSymbols.Length; i++)
                symbols.Add(new MessageElementString(mSymbols[i]));

            message["data"] = new MessageElementObject()
            {
                { mMode == DXFeedSubscribeMode.Add ? "add" : "remove", 
                           new MessageElementObject() { { "Quote", symbols } } },
            };
            message["clientId"] = new MessageElementString(mClientId);
        }
    }
}
