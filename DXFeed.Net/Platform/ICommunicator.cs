using System;

namespace DXFeed.Net.Platform
{
    /// <summary>
    /// The interface to a communicator
    /// </summary>
    public interface ICommunicator : IDisposable
    {
        /// <summary>
        /// Event raised when a sender is started
        /// </summary>
        event EventHandler<CommunicatorEventArgs>? SenderStarted;

        /// <summary>
        /// Event raised when a sender is stopped
        /// </summary>
        event EventHandler<CommunicatorEventArgs>? SenderStopped;

        /// <summary>
        /// Event raised when a receiver is started
        /// </summary>
        event EventHandler<CommunicatorEventArgs>? ReceiverStarted;

        /// <summary>
        /// Event raised when a receiver is stopped
        /// </summary>
        event EventHandler<CommunicatorEventArgs>? ReceiverStopped;

        /// <summary>
        /// Event raised when an exception happen in a sender or receiver
        /// </summary>
        event EventHandler<CommunicatorExceptionEventArgs>? ExceptionRaised;
        
        /// <summary>
        /// Event raised when a message is send
        /// </summary>
        event EventHandler<CommunicatorMessageEventArgs>? MessageSent;

        /// <summary>
        /// Event raised when a message is received
        /// </summary>
        event EventHandler<CommunicatorMessageEventArgs>? MessageReceived;

        /// <summary>
        /// Sends a message
        /// </summary>
        /// <param name="message"></param>
        void Send(string message);
    }


}
