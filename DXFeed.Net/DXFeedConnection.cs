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
        private readonly Thread mHeartbeatSender;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="communicator"></param>
        /// <param name="disposeCommunicator"></param>
        public DXFeedConnection(ICommunicator communicator, bool disposeCommunicator)
        {
            mCommunicator = communicator;
            mDisposeCommunicator = disposeCommunicator;
            mCommunicator.SenderStarted += BackgroundStatus;
            mCommunicator.ReceiverStarted += BackgroundStatus;
            mCommunicator.SenderStopped += BackgroundStatus;
            mCommunicator.ReceiverStopped += BackgroundStatus;
            mCommunicator.ExceptionRaised += ExceptionRaised;
            mCommunicator.MessageReceived += MessageReceived;
            mHeartbeatSender = new Thread(HeartbeatSender)
            {
                IsBackground = true
            };
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
                mAuthorizationSend = false;
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
        private bool mAuthorizationSend;

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
                    return mAuthorizationSend ? DXFeedConnectionState.Connecting : DXFeedConnectionState.ReadyToConnect;
                else if (mHeartbeatResponseReceived)
                    return DXFeedConnectionState.Connecting;
                else
                    return DXFeedConnectionState.Ready;
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

        private void ProcessMessage(IMessageElementObject @object)
        {
            var message = DXFeedResponseParser.Parse(@object);
            if (message == null)
                return;
            if (message is DXFeedAuthorizeResponse authorize)
            {
                mAuthorizationSend = false;
                if (authorize.Successful && !string.IsNullOrEmpty(authorize.ClientId))
                {
                    ClientId = authorize.ClientId;
                    if (authorize.Advice != null && authorize.Advice.Timeout != null)
                        HeartbitPeriod = authorize.Advice.Timeout.Value;
                    
                    //TBD: send first heartbeat!
                }
            }
            else if (message is DXFeedHeartbeatResponse heartbeat)
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
                    ClientId = null;
                    CallStatusChange();
                }
            }
        }

        /// <summary>
        /// Invokes status change
        /// </summary>
        private void CallStatusChange()
        {
            mListeners.CallStatusChange(this);
            
            if (State == DXFeedConnectionState.Ready && !mHeartbeatSender.IsAlive)
                mHeartbeatSender.Start();
            else if (State != DXFeedConnectionState.Ready && mHeartbeatSender.IsAlive)
                mStopHeartbit.Set();
        }

        /// <summary>
        /// The event to send a heartbeat message
        /// </summary>
        private readonly AutoResetEvent mStopHeartbit = new AutoResetEvent(false);
        
        /// <summary>
        /// The period to send heartbit message in milliseconds
        /// 
        /// dxfeed recommends 30 seconds
        /// </summary>
        public int HeartbitPeriod { get; set; } = 30000;

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
                        //TBD: send heartbeat
                    }
                }
                catch (Exception e)
                {
                    mListeners.CallException(this, e);
                }
            }
        }

        //TBD: Authorization method

    }
}
