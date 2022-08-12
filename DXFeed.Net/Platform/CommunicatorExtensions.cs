using DXFeed.Net.Message;

namespace DXFeed.Net.Platform
{
    /// <summary>
    /// Extensions for communicator to send messages
    /// </summary>
    public static class CommunicatorExtensions
    {
        /// <summary>
        /// Sends a dxfeed message
        /// </summary>
        /// <param name="communicator"></param>
        /// <param name="array"></param>
        public static void Send(this ICommunicator communicator, IMessageElementArray array) => communicator.Send(array.SerializeToString());

        /// <summary>
        /// Packs an object into array and sends as a dxfeed message.
        /// </summary>
        /// <param name="communicator"></param>
        /// <param name="object"></param>
        public static void Send(this ICommunicator communicator, IMessageElementObject @object) => communicator.Send(new MessageElementArray() { @object });
    }

}
