using DXFeed.Net.Message;
using System.Threading;

namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Base class for all messages
    /// </summary>
    public abstract class DXFeedMessage : IDXFeedMessage
    {
        private readonly string mChannel;
        private readonly DXFeedMessageType mMessageType;
        private static int mID = 0;

        /// <summary>
        /// Returns message type
        /// </summary>
        public DXFeedMessageType MessageType => mMessageType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mChannel"></param>
        /// <param name="mMessageType"></param>
        protected DXFeedMessage(string mChannel, DXFeedMessageType mMessageType)
        {
            this.mChannel = mChannel;
            this.mMessageType = mMessageType;
        }

        /// <summary>
        /// Gets message object
        /// </summary>
        /// <returns></returns>
        public IMessageElementObject ToMessage()
        {
            int id = Interlocked.Increment(ref mID);
            var message = new MessageElementObject()
            {
                { "id", new MessageElementString($"{id}") },
                { "channel", new MessageElementString(mChannel) },
            };
            ConfigureMessage(message);
            return message;
        }

        /// <summary>
        /// Override this method to configure the message details
        /// </summary>
        /// <param name="message"></param>
        protected virtual void ConfigureMessage(IMessageElementObject message)
        {
            return;
        }
    }
}
