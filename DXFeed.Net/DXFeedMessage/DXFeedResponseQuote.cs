using DXFeed.Net.Message;
using System;
using System.Collections.Generic;
using DXFeed.Net.Platform;
using System.Collections;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// The response with a quote
    /// </summary>
    public class DXFeedResponseQuote : DXFeedResponse, IReadOnlyCollection<DXFeedResponseQuote.Quote>
    {
        private readonly List<Quote> mQuotes = new List<Quote>();

        /// <summary>
        /// First of the quotes
        /// </summary>
        public Quote? FirstQuote => mQuotes.Count > 0 ? mQuotes[0] : null;

        /// <summary>
        /// The quotes quote
        /// </summary>
        public int Count => mQuotes.Count;

        /// <summary>
        /// Gets quote by its index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Quote this[int index] => mQuotes[index];

        /// <summary>
        /// One quote
        /// </summary>
        public sealed class Quote
        {
            public string? Symbol { get; }
            public DateTime? BidTime { get; }
            public DateTime? AskTime { get; }
            public double? Bid { get; }
            public double? Ask { get; }
            public double? BidSize { get; }
            public double? AskSize { get; }
            public string? BidExchangeCode { get; }
            public string? AskExchangeCode { get; }

            internal Quote(List<string> fieldDictionary, IMessageElementArray values, int valuesBase = 0) 
            {
                for (int i = 0; i < fieldDictionary.Count; i++)
                {
                    if (i + valuesBase < values.Length)
                    {
                        var value = values[i + valuesBase];
                        switch (fieldDictionary[i])
                        {
                            case "eventSymbol":
                                {
                                    if (value.AsString(out var s))
                                        Symbol = s;
                                }
                                break;
                            case "bidTime":
                                {
                                    if (value.AsLong(out var l))
                                        BidTime = l.FromDXFeed();
                                }
                                break;
                            case "bidExchangeCode":
                                {
                                    if (value.AsString(out var s))
                                        BidExchangeCode = s;
                                }
                                break;
                            case "bidPrice":
                                {
                                    if (value.AsDouble(out var r))
                                        Bid = r;
                                }
                                break;
                            case "bidSize":
                                {
                                    if (value.AsDouble(out var r))
                                        BidSize = r;
                                }
                                break;
                            case "askTime":
                                {
                                    if (value.AsLong(out var l))
                                        AskTime = l.FromDXFeed();
                                }
                                break;
                            case "askExchangeCode":
                                {
                                    if (value.AsString(out var s))
                                        AskExchangeCode = s;
                                }
                                break;
                            case "askPrice":
                                {
                                    if (value.AsDouble(out var r))
                                        Ask = r;
                                }

                                break;
                            case "askSize":
                                {
                                    if (value.AsDouble(out var r))
                                        AskSize = r;
                                }
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldDictionary"></param>
        /// <param name="values"></param>
        /// <param name="valuesBase"></param>
        public DXFeedResponseQuote(List<string> fieldDictionary, IMessageElementArray values) : base(DXFeedResponseType.Quote)
        {
            for (int i = 0; i < values.Length; i += fieldDictionary.Count)
            {
                var quote = new Quote(fieldDictionary, values, i);
                mQuotes.Add(quote);
            }
        }

        public IEnumerator<Quote> GetEnumerator() => mQuotes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => mQuotes.GetEnumerator();
    }
}
