using DXFeed.Net;
using DXFeed.Net.DXFeedMessage;
using DXFeed.Net.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXFeed.Demo
{
    internal class Listener : IDXFeedConnectionListener
    {
        private bool subscribed = false;

        public void OnException(IDXFeedConnection connection, Exception exception)
        {
            Console.WriteLine("Exception in communicator: {0}", exception.Message); 
        }

        public void OnStatusChanged(IDXFeedConnection connection, DXFeedConnectionState state)
        {
            if (state == DXFeedConnectionState.ReadyToSubscribe)
            {
                if (!subscribed)
                {
                    connection.SubscribeForQuotes(new string[] { "AAPL", "MSFT" });
                    Console.WriteLine("Sending subscription....");
                    subscribed = true;
                }
            }
            Console.WriteLine("Status changed: {0}", state);
        }

        public void OnQuoteReceived(IDXFeedConnection connection, DXFeedResponseQuote quote)
        {
            foreach (var q in quote)
                Console.WriteLine("{0} {1}@{2}:{5} {3}@{4}:{6}", q.Symbol, q.Bid, q.BidTime, q.Ask, q.AskTime, q.BidSize, q.AskSize);
        }
    }
}
