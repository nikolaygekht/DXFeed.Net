using DXFeed.Net.Message;
using DXFeed.Net.Platform;
using System;

namespace DXFeed.Net.DXFeedMessage
{

    /// <summary>
    /// Subscribe for quotes message
    /// </summary>
    public class DXFeedMessageSubscribeForCandles : DXFeedMessage
    {
        private readonly DXFeedCandleRequest[] mRequests;
        private readonly string mClientId;
        private readonly DXFeedSubscribeMode mMode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="symbol"></param>
        /// <param name="clientId"></param>
        public DXFeedMessageSubscribeForCandles(DXFeedSubscribeMode mode, DXFeedCandleRequest request, string clientId)
            : this(mode, new DXFeedCandleRequest[] { request }, clientId)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="token"></param>
        public DXFeedMessageSubscribeForCandles(DXFeedSubscribeMode mode, DXFeedCandleRequest[] requests, string clientId) : base("/service/sub", DXFeedMessageType.SubscribeForCandles)
        {
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("Client id is empty", nameof(clientId));

            if (requests == null)
                throw new ArgumentNullException(nameof(requests));

            if (requests.Length == 0)
                throw new ArgumentException("The symbol list is empty", nameof(requests));

            mRequests = new DXFeedCandleRequest[requests.Length];
            requests.CopyTo(mRequests, 0);

            mClientId = clientId;
            mMode = mode;
        }

        protected override void ConfigureMessage(IMessageElementObject message)
        {
            var requests = new MessageElementArray();
            for (int i = 0; i < mRequests.Length; i++)
            {
                if (mMode == DXFeedSubscribeMode.Add)
                {
                    var obj = new MessageElementObject()
                    {
                        { "eventSymbol", new MessageElementString(mRequests[i].Symbol.ToString()) }
                    };

                    var from = mRequests[i].From;
                    if (from != null)
                        obj["fromTime"] = new MessageElementLong(from.Value.ToDXFeed());

                    requests.Add(obj);
                }
                else
                {
                    requests.Add(new MessageElementString(mRequests[i].Symbol.ToString()));
                }
            }

            message["data"] = new MessageElementObject()
            {
                { mMode == DXFeedSubscribeMode.Add ? "addTimeSeries" : "removeTimeSeries",
                           new MessageElementObject() { { "Candle", requests } } },
            };
            message["clientId"] = new MessageElementString(mClientId);
        }
    }

}
