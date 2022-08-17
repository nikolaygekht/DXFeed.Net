using System;
using System.Text.RegularExpressions;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// The symbol of a candle
    /// </summary>
    public sealed class DXFeedCandleSymbol : IEquatable<DXFeedCandleSymbol>, IEquatable<string>
    {
        public static DXFeedCandleSymbol Invalid => gInvalid;

        /// <summary>
        /// The symbol
        /// </summary>
        public string Symbol { get; }
        /// <summary>
        /// The aggregation period (e.g. d, 4h, 1m etc...)
        /// </summary>
        public string AggregationPeriod { get; }

        public DXFeedCandleSymbol(string symbol, string aggregationPeriod)
        {
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException("Symbol must not be null or empty", nameof(symbol));

            if (string.IsNullOrEmpty(aggregationPeriod))
                throw new ArgumentException("Aggregation period must not be null or empty", nameof(symbol));

            Symbol = symbol;
            AggregationPeriod = aggregationPeriod;
        }

        private readonly static Regex gParseRegex = new Regex(@"^\s*(\.?[\w\d\:\/\&]+)\{=([\d\w]+)\}\s*$");
        private readonly static DXFeedCandleSymbol gInvalid = new DXFeedCandleSymbol("invalid", "invalid");

        /// <summary>
        /// Parse candle
        /// </summary>
        /// <param name="text"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static bool TryParse(string text, out DXFeedCandleSymbol symbol)
        {
            if (text == null)
            {
                symbol = gInvalid;
                return false;
            }

            var m = gParseRegex.Match(text);
            if (!m.Success)
            {
                symbol = gInvalid;
                return false;
            }

            symbol = new DXFeedCandleSymbol(m.Groups[1].Value, m.Groups[2].Value);
            return true;
        }

        public override string ToString()
        {
            return $"{Symbol}{{={AggregationPeriod}}}";
        }

        public bool Equals(DXFeedCandleSymbol symbol)
        {
            if (symbol == null)
                return false;
            return Symbol == symbol.Symbol && AggregationPeriod == symbol.AggregationPeriod;
        }

        public bool Equals(string symbol) => ToString() == symbol;

        override public bool Equals(object obj)
        {
            if (obj is DXFeedCandleSymbol symbol)
                return Equals(symbol);
            if (obj is string text)
                return Equals(text);
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(Symbol, AggregationPeriod);
    }
}
