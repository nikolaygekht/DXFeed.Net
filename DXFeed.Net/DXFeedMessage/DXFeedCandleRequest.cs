using System;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// The symbol of a candle
    /// </summary>
    public class DXFeedCandleRequest
    {
        /// <summary>
        /// The symbol requested
        /// </summary>
        public DXFeedCandleSymbol Symbol { get; }

        /// <summary>
        /// The date and time to request the candles from
        /// </summary>
        public DateTime? From { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="from"></param>
        public DXFeedCandleRequest(DXFeedCandleSymbol symbol, DateTime? from = null)
        {
            Symbol = symbol;
            From = from;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="from"></param>
        public DXFeedCandleRequest(string symbol, string aggregationPeriod, DateTime? from)
            : this(new DXFeedCandleSymbol(symbol, aggregationPeriod), from)
        {
        }
    }

}
