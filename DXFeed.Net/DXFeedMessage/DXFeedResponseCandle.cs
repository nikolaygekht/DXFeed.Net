using DXFeed.Net.Message;
using System;
using System.Collections.Generic;
using DXFeed.Net.Platform;
using System.Collections;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// The candle(s) response
    /// </summary>
    public class DXFeedResponseCandle : DXFeedResponse, IReadOnlyCollection<DXFeedResponseCandle.Candle>
    {
        private readonly List<Candle> mCandles = new List<Candle>();

        /// <summary>
        /// First of the quotes
        /// </summary>
        public Candle? FirstCandle => mCandles.Count > 0 ? mCandles[0] : null;

        /// <summary>
        /// The quotes quote
        /// </summary>
        public int Count => mCandles.Count;

        /// <summary>
        /// Gets quote by its index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Candle this[int index] => mCandles[index];

        public IEnumerator<Candle> GetEnumerator() => mCandles.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #region Candle
        /// <summary>
        /// Candle
        /// </summary>
        public class Candle
        {
            public DXFeedCandleSymbol Symbol { get; }
            public long Index { get; }
            public DateTime Time { get; }
            public int Count { get; }
            public int Sequence { get; }
            public double Open { get; }
            public double Close { get; }
            public double High { get; }
            public double Low { get; }
            public double Volume { get; }
            public double VWAP { get; }
            public double BidVolume { get; }
            public double AskVolume { get; }
            public double Volatility { get; }
            public double OpenInterest { get; }

            public Candle(DXFeedCandleSymbol symbol, long index, DateTime time, int count, int sequence, double open, double close, double high, double low, double volume, double vWAP, double bidVolume, double askVolume, double volatility, double openInterest)
            {
                Symbol = symbol;
                Index = index;
                Time = time;
                Count = count;
                Sequence = sequence;
                Open = open;
                Close = close;
                High = high;
                Low = low;
                Volume = volume;
                VWAP = vWAP;
                BidVolume = bidVolume;
                AskVolume = askVolume;
                Volatility = volatility;
                OpenInterest = openInterest;
            }

            internal Candle(List<string> fieldDictionary, IMessageElementArray values, int valuesBase = 0)
            {
                Symbol = DXFeedCandleSymbol.Invalid;

                for (int i = 0; i < fieldDictionary.Count; i++)
                {
                    if (i + valuesBase < values.Length)
                    {
                        var value = values[i + valuesBase];
                        switch (fieldDictionary[i])
                        {
                            case "eventSymbol":
                                {
                                    if (value.AsString(out var s) && DXFeedCandleSymbol.TryParse(s, out var ss))
                                        Symbol = ss;
                                }
                                break;
                            case "time":
                                {
                                    if (value.AsLong(out var l))
                                        Time = l.FromDXFeed();
                                }
                                break;
                            case "index":
                                {
                                    if (value.AsLong(out var l))
                                        Index = l;
                                }
                                break;
                            case "count":
                                {
                                    if (value.AsInteger(out var l))
                                        Count = l;
                                }
                                break;
                            case "sequence":
                                {
                                    if (value.AsInteger(out var l))
                                        Sequence = l;
                                }
                                break;
                            case "open":
                                {
                                    if (value.AsDouble(out var l))
                                        Open = l;
                                }
                                break;
                            case "close":
                                {
                                    if (value.AsDouble(out var l))
                                        Close = l;
                                }
                                break;
                            case "high":
                                {
                                    if (value.AsDouble(out var l))
                                        High = l;
                                }
                                break;
                            case "low":
                                {
                                    if (value.AsDouble(out var l))
                                        Low = l;
                                }
                                break;
                            case "volume":
                                {
                                    if (value.AsDouble(out var l))
                                        Volume = l;
                                }
                                break;
                            case "bidVolume":
                                {
                                    if (value.AsDouble(out var l))
                                        BidVolume = l;
                                }
                                break;
                            case "askVolume":
                                {
                                    if (value.AsDouble(out var l))
                                        AskVolume = l;
                                }
                                break;
                            case "vwap":
                                {
                                    if (value.AsDouble(out var l))
                                        VWAP = l;
                                }
                                break;
                            case "impVolatility":
                                {
                                    if (value.AsDouble(out var l))
                                        Volatility = l;
                                }
                                break;
                            case "openInterest":
                                {
                                    if (value.AsDouble(out var l))
                                        OpenInterest = l;
                                }
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldDictionary"></param>
        /// <param name="values"></param>
        /// <param name="valuesBase"></param>
        public DXFeedResponseCandle(List<string> fieldDictionary, IMessageElementArray values) : base(DXFeedResponseType.Candle)
        {
            for (int i = 0; i < values.Length; i += fieldDictionary.Count)
            {
                var candle = new Candle(fieldDictionary, values, i);
                mCandles.Add(candle);
            }
        }
    }

}


