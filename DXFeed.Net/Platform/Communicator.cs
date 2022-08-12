using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DXFeed.Net.Platform
{

    /// <summary>
    /// The communicator implementation
    /// </summary>
    public sealed class Communicator : ICommunicator
    {
        private readonly bool mDisposeTransport;
        private readonly ITransport mTransport;
        private readonly ConcurrentQueue<string> mSendQueue = new ConcurrentQueue<string>();
        private readonly AutoResetEvent mSent = new AutoResetEvent(false);
        private readonly ManualResetEvent mExit = new ManualResetEvent(false);
        private readonly CommunicatorEventArgs mThisCommunicatorArgs;
        private readonly Thread mSenderThread, mReceiverThread;
        private bool mExiting = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="transport"></param>
        public Communicator(ITransport? transport, bool disposeTransport)
        {
            if (transport == null)
                throw new ArgumentNullException(nameof(transport));

            mTransport = transport;
            mDisposeTransport = disposeTransport;
            mThisCommunicatorArgs = new CommunicatorEventArgs(this);

            mSenderThread = new Thread(Sender)
            {
                IsBackground = true,
            };

            mReceiverThread = new Thread(Receiver)
            {
                IsBackground = true,
            };
        }

        public void Dispose()
        {
            if (mSenderThread.IsAlive || mReceiverThread.IsAlive)
            {
                mExiting = true;
                mExit.Set();
                Thread.Yield();
            }

            if (mDisposeTransport)
                mTransport.Dispose();           
        }

        /// <summary>
        /// Checks whether the communicator is active
        /// </summary>
        public bool Active
        {
            get
            {
                return mTransport.State == TransportState.Open && mReceiverThread.IsAlive && mSenderThread.IsAlive;
            }
        }


        public void Start()
        {
            mSenderThread.Start();
            mReceiverThread.Start();
        }

        /// <summary>
        /// Event raised when a sender is started
        /// </summary>
        public event EventHandler<CommunicatorEventArgs>? SenderStarted;

        /// <summary>
        /// Event raised when a sender is stopped
        /// </summary>
        public event EventHandler<CommunicatorEventArgs>? SenderStopped;

        /// <summary>
        /// Event raised when a receiver is started
        /// </summary>
        public event EventHandler<CommunicatorEventArgs>? ReceiverStarted;

        /// <summary>
        /// Event raised when a receiver is stopped
        /// </summary>
        public event EventHandler<CommunicatorEventArgs>? ReceiverStopped;

        /// <summary>
        /// Event raised when an exception happen in a sender or receiver
        /// </summary>
        public event EventHandler<CommunicatorExceptionEventArgs>? ExceptionRaised;

        /// <summary>
        /// Event raised when a message is send
        /// </summary>
        public event EventHandler<CommunicatorMessageEventArgs>? MessageSent;

        /// <summary>
        /// Event raised when a message is received
        /// </summary>
        public event EventHandler<CommunicatorMessageEventArgs>? MessageReceived;

        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="message"></param>
        public void Send(string? message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            mSendQueue.Enqueue(message);
            mSent.Set();
        }

        /// <summary>
        /// Closes the transport
        /// </summary>
        public void Close()
        {
            mExiting = true;
            mExit.Set();
            Thread.Yield(); 
            mTransport.Close();
            DateTime maxWait = DateTime.Now.AddSeconds(1);
            while (DateTime.Now < maxWait)
            {
                if (!mSenderThread.IsAlive && !mReceiverThread.IsAlive)
                    break;
            }
        }

        /// <summary>
        /// The receiver process
        /// </summary>
        private void Receiver()
        {
            ReceiverStarted?.Invoke(this, mThisCommunicatorArgs);
            
            while (!mExiting)
            {
                try
                {
                    var message = mTransport.Receive();
                    if (message != null)
                        MessageReceived?.Invoke(this, new CommunicatorMessageEventArgs(this, message));
                }
                catch (Exception e)
                {
                    if (mExiting)
                        break;
                    ExceptionRaised?.Invoke(this, new CommunicatorExceptionEventArgs(this, e));
                    if (mTransport.State != TransportState.Open)
                        break;
                }
            }           
            ReceiverStopped?.Invoke(this, mThisCommunicatorArgs);
        }
        
        /// <summary>
        /// The sender process
        /// </summary>
        private void Sender()
        {
            var handles = new WaitHandle[] { mSent, mExit };

            SenderStarted?.Invoke(this, mThisCommunicatorArgs);
            while (true)
            {
                if (mSendQueue.IsEmpty)
                    WaitHandle.WaitAny(handles, 1000);
                if (mExiting)
                    break;

                if (!mSendQueue.IsEmpty && mSendQueue.TryDequeue(out var message))
                {
                    try
                    {
                        mTransport.Send(message);
                        MessageSent?.Invoke(this, new CommunicatorMessageEventArgs(this, message));
                    }
                    catch (Exception e)
                    {
                        if (mExiting)
                            break;

                        ExceptionRaised?.Invoke(this, new CommunicatorExceptionEventArgs(this, e));
                        
                        if (mTransport.State != TransportState.Open)
                            break;
                    }
                }
            }
            SenderStopped?.Invoke(this, mThisCommunicatorArgs);
        }
    }


}
