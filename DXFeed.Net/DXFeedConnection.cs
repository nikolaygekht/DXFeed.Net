using DXFeed.Net.Message;
using DXFeed.Net.Platform;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
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
                ClientId = null;

            mListeners.CallStatusChange(this);
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

        /// <summary>
        /// Current connection status.
        /// </summary>
        public DXFeedConnectionStatus Status
        {
            get
            {
                if (!mCommunicator.Active)
                    return DXFeedConnectionStatus.Disconnected;
                if (string.IsNullOrEmpty(ClientId))
                    return DXFeedConnectionStatus.Connected;
                else
                    return DXFeedConnectionStatus.Ready;
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
            }
            catch (Exception e)
            {
                mListeners.CallException(this, e);
                return ;
            }


        }

    }
}
