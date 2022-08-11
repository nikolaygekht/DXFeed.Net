using System;

namespace DXFeed.Net.Platform
{
    /// <summary>
    /// Arguments for <see cref="ICommunicator" events />
    /// </summary>
    public class CommunicatorExceptionEventArgs : CommunicatorEventArgs
    {
        /// <summary>
        /// The exception raised
        /// </summary>
        public Exception Exception { get; }

        internal CommunicatorExceptionEventArgs(ICommunicator communicator, Exception exception) : base(communicator) 
        {
            Exception = exception;
        }
    }
}
