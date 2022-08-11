namespace DXFeed.Net.Platform
{
    /// <summary>
    /// Arguments for <see cref="ICommunicator" events />
    /// </summary>
    public class CommunicatorMessageEventArgs : CommunicatorEventArgs
    {
        /// <summary>
        /// The message received
        /// </summary>
        public string Message { get; }

        internal CommunicatorMessageEventArgs(ICommunicator communicator, string message) : base(communicator)
        {
            Message = message;
        }
    }
}
