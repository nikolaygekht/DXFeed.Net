using System;

namespace DXFeed.Net.Platform
{
    /// <summary>
    /// Arguments for <see cref="ICommunicator" events />
    /// </summary>
    public class CommunicatorEventArgs : EventArgs
    {
        /// <summary>
        /// The communicator
        /// </summary>
        public ICommunicator Communicator { get; }

        internal CommunicatorEventArgs(ICommunicator communicator)
        {
            Communicator = communicator;
        }
    }
}
