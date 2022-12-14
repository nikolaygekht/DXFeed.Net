using DXFeed.Net.DXFeedMessage;
using DXFeed.Net.Message;
using DXFeed.Net.Platform;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DXFeed.Net
{
    /// <summary>
    /// The connection class
    /// </summary>
    public class DXFeedConnection : IDXFeedConnection
    {
        private readonly ICommunicator mCommunicator;
        private readonly bool mDisposeCommunicator;
        private Thread? mHeartbeatSender;
        private readonly string mToken;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="communicator"></param>
        /// <param name="disposeCommunicator"></param>
        public DXFeedConnection(string? token, ICommunicator communicator, bool disposeCommunicator)
        {
            mToken = token ?? throw new ArgumentNullException(nameof(token));
            mCommunicator = communicator ?? throw new ArgumentNullException(nameof(communicator));
            mDisposeCommunicator = disposeCommunicator;
            SubscribeToCommunicator();
            if (mCommunicator.Active)
                Task.Run(() => Authorize());   //start authorization in background
        }

        /// <summary>
        /// Constructor for debugging purposes. Creates initialized session.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="clientId"></param>
        /// <param name="timeout"></param>
        /// <param name="communicator"></param>
        /// <param name="disposeCommunicator"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        internal DXFeedConnection(string token, string clientId, int timeout, ICommunicator communicator, bool disposeCommunicator)
        {
            if (communicator == null)
                throw new ArgumentNullException(nameof(communicator));

            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("ClientId cannot be null or empty", nameof(clientId));

            if (communicator.GetType().Assembly.GetName().Name != "DynamicProxyGenAssembly2")
                throw new ArgumentException("Communicator must be generated by DynamicProxyGenAssembly2", nameof(communicator));

            if (!communicator.Active)
                throw new ArgumentException("Communicator must be active at start", nameof(communicator));

            mToken = token ?? throw new ArgumentNullException(nameof(token));

            ClientId = clientId;
            mHeartbeatResponseReceived = true;

            mCommunicator = communicator;
            mDisposeCommunicator = disposeCommunicator;
            SubscribeToCommunicator();
            HeartbitPeriod = timeout;

            mHeartbeatSender = new Thread(HeartbeatSender)
            {
                IsBackground = true,
            };
            mHeartbeatSender.Start();
            while (!mHeartbeatSender.IsAlive) Thread.Yield();
        }

        private void SubscribeToCommunicator()
        {
            mCommunicator.SenderStarted += BackgroundStatus;
            mCommunicator.ReceiverStarted += BackgroundStatus;
            mCommunicator.SenderStopped += BackgroundStatus;
            mCommunicator.ReceiverStopped += BackgroundStatus;
            mCommunicator.ExceptionRaised += ExceptionRaised;
            mCommunicator.MessageReceived += MessageReceived;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~DXFeedConnection()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The method to be overridden to dispose all disposable resource
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (mDisposeCommunicator)
                mCommunicator.Dispose();

            if (mHeartbeatSender != null && mHeartbeatSender.IsAlive)
                mStopHeartbit.Set();
        }

        /// <summary>
        /// Called when sender or received is stopped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void BackgroundStatus(object sender, CommunicatorEventArgs args)
        {
            if (!mCommunicator.Active)
            {
                ClientId = null;
                mHeartbeatResponseReceived = false;
                mAuthorizationSent = false;
            }
            CallStatusChange();
        }

        /// <summary>
        /// Called by communicator when an exception is raised in the sender or received
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ExceptionRaised(object sender, CommunicatorExceptionEventArgs args)
        {
            mListeners.CallException(this, args.Exception);
        }

        /// <summary>
        /// Client Id in the current session 
        /// </summary>
        public string? ClientId { get; private set; }

        private bool mHeartbeatResponseReceived;
        private bool mAuthorizationSent;

        /// <summary>
        /// Current connection status.
        /// </summary>
        public DXFeedConnectionState State
        {
            get
            {
                if (!mCommunicator.Active)
                    return DXFeedConnectionState.Disconnected;
                if (string.IsNullOrEmpty(ClientId))
                    return mAuthorizationSent ? DXFeedConnectionState.Connecting : DXFeedConnectionState.ReadyToConnect;
                else if (!mHeartbeatResponseReceived)
                    return DXFeedConnectionState.Connecting;
                else
                    return DXFeedConnectionState.ReadyToSubscribe;
            }
        }

        /// <summary>
        /// The collection of listeners subscribed to the connection
        /// </summary>
        private readonly DXFeedConnectionListenerCollection mListeners = new DXFeedConnectionListenerCollection();

        /// <summary>
        /// Subscribe listener 
        /// </summary>
        /// <param name="listener"></param>
        public void SubscribeListener(IDXFeedConnectionListener listener)
            => mListeners.SubscribeListener(listener);

        /// <summary>
        /// Unsubscribe listener
        /// </summary>
        /// <param name="listener"></param>
        public void UnsubscribeListener(IDXFeedConnectionListener listener)
            => mListeners.UnsubscribeListener(listener);

        /// <summary>
        /// The event raised when a message is received 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void MessageReceived(object sender, CommunicatorMessageEventArgs args)
        {
            IMessageElement element;
            try
            {
                element = args.Message.DeserializeToMessage();
                if (element.ElementType == MessageElementType.Array &&
                    element is IMessageElementArray array &&
                    array.Length == 1)
                    element = array[0];

                if (element.ElementType == MessageElementType.Object &&
                    element is IMessageElementObject @object)
                    ProcessMessage(@object);
            }
            catch (Exception e)
            {
                mListeners.CallException(this, e);
            }
        }

        /// <summary>
        /// Invokes the authorization
        /// </summary>
        private void Authorize()
        {
            mAuthorizationSent = true;
            if (mHeartbeatSender == null || !mHeartbeatSender.IsAlive)
            {
                mHeartbeatSender = new Thread(HeartbeatSender)
                {
                    IsBackground = true,
                };
                mHeartbeatSender.Start();
                while (!mHeartbeatSender.IsAlive) Thread.Yield();
            }

            var message = new DXFeedMessageAuthorize(mToken);
            mCommunicator.Send(message.ToMessage());
        }

        /// <summary>
        /// Detects the type of the message and routes it to proper processor it
        /// </summary>
        /// <param name="object"></param>
        private void ProcessMessage(IMessageElementObject @object)
        {
            var message = DXFeedResponseParser.Parse(@object);
            if (message == null)
                return;

            switch (message)
            {
                case DXFeedResponseAuthorize authorize:
                    Process(authorize);
                    break;
                case DXFeedResponseHeartbeat heartbeat:
                    Process(heartbeat);
                    break;
                case DXFeedResponseQuote quote:
                    mListeners.CallOnQuote(this, quote);
                    break;
                case DXFeedResponseCandle candle:
                    mListeners.CallOnCandle(this, candle);
                    break;
            }
        }

        /// <summary>
        /// Process response for an authorization message
        /// </summary>
        /// <param name="authorize"></param>
        private void Process(DXFeedResponseAuthorize authorize)
        {
            if (authorize.Successful)
            {
                if (!string.IsNullOrEmpty(authorize.ClientId))
                {
                    bool clientIdWasEmpty = string.IsNullOrEmpty(ClientId);
                    ClientId = authorize.ClientId;

                    if (clientIdWasEmpty)
                    {
                        mHeartbeatResponseReceived = false;
                        var heartbeat = new DXFeedMessageHeartbeat(ClientId);
                        mCommunicator.Send(heartbeat.ToMessage());
                    }
                }
                if (authorize.Advice != null && authorize.Advice.Timeout != null)
                    HeartbitPeriod = authorize.Advice.Timeout.Value;
            }
        }

        private void Process(DXFeedResponseHeartbeat heartbeat)
        {
            if (heartbeat.Successful)
            {
                mHeartbeatResponseReceived = true;
                if (!string.IsNullOrEmpty(heartbeat.ClientId))
                    ClientId = heartbeat.ClientId;
                CallStatusChange();
            }
            else
            {
                mAuthorizationSent = false;
                mHeartbeatResponseReceived = false;
                ClientId = null;
                CallStatusChange();
            }

        }

        /// <summary>
        /// Invokes status change
        /// </summary>
        private void CallStatusChange()
        {
            mListeners.CallStatusChange(this);
            var state = State;

            if (state == DXFeedConnectionState.Disconnected && mHeartbeatSender != null && mHeartbeatSender.IsAlive)
                mStopHeartbit.Set();

            if (state == DXFeedConnectionState.ReadyToConnect && !mAuthorizationSent)
            {
                mAuthorizationSent = true;
                Task.Run(() => Authorize());
            }

        }

        /// <summary>
        /// The event to send a heartbeat message
        /// </summary>
        private readonly AutoResetEvent mStopHeartbit = new AutoResetEvent(false);

        /// <summary>
        /// The period to send heartbit message in milliseconds
        /// 
        /// This value will be updated later from the authorization response
        /// </summary>
        public int HeartbitPeriod { get; set; } = 5000;

        /// <summary>
        /// The background thread to send heartbeat
        /// </summary>
        private void HeartbeatSender()
        {
            while (true)
            {
                try
                {
                    if (mStopHeartbit.WaitOne(HeartbitPeriod))
                        break;

                    if (!string.IsNullOrEmpty(ClientId))
                    {
                        var message = new DXFeedMessageHeartbeat(ClientId);
                        mCommunicator.Send(message.ToMessage());
                    }
                }
                catch (Exception e)
                {
                    mListeners.CallException(this, e);
                }
            }
        }

        /// <summary>
        /// Associated communicator
        /// </summary>
        public ICommunicator Communicator => mCommunicator;

        /// <summary>
        /// Subscribe for quotes
        /// </summary>
        /// <param name="symbols"></param>
        public void SubscribeForQuotes(string[] symbols)
        {
            if (ClientId != null)
            {
                var message = new DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode.Add, symbols, ClientId);
                mCommunicator.Send(message.ToMessage());
            }
        }

        /// <summary>
        /// Unsubscribe from quotes
        /// </summary>
        public void UnsubscribeFromQuotes(string[] symbols)
        {
            if (ClientId != null)
            {
                var message = new DXFeedMessageSubscribeForQuotes(DXFeedSubscribeMode.Remove, symbols, ClientId);
                mCommunicator.Send(message.ToMessage());
            }
        }

        /// <summary>
        /// Subscribe for candles
        /// </summary>
        /// <param name="candles"></param>
        public void SubscribeForCandles(DXFeedCandleRequest[] candles)
        {
            if (ClientId != null)
            {
                var message = new DXFeedMessageSubscribeForCandles(DXFeedSubscribeMode.Add, candles, ClientId);
                mCommunicator.Send(message.ToMessage());
            }
        }

        /// <summary>
        /// Unsubscribe from candles
        /// </summary>
        public void UnsubscribeFromCandles(DXFeedCandleRequest[] candles)
        {
            if (ClientId != null)
            {
                var message = new DXFeedMessageSubscribeForCandles(DXFeedSubscribeMode.Remove, candles, ClientId);
                mCommunicator.Send(message.ToMessage());
            }
        }
    }
}
