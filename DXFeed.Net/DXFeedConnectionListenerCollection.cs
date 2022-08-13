using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DXFeed.Net
{
    /// <summary>
    /// Collection of the listeners
    /// </summary>
    internal class DXFeedConnectionListenerCollection
    {
        private readonly object mListenersMutex = new object();
        private readonly LinkedList<IDXFeedConnectionListener> mListeners = new LinkedList<IDXFeedConnectionListener>();

        /// <summary>
        /// Returns number of listeners
        /// </summary>
        public int Count
        {
            get
            {
                lock (mListenersMutex)
                    return mListeners.Count;
            }
        }

        /// <summary>
        /// Checks whether the listener is subscribed
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool IsSubscribed(IDXFeedConnectionListener listener)
        {
            lock (mListenersMutex)
                return mListeners.Contains(listener);
        }

        /// <summary>
        /// Subscribe listener 
        /// </summary>
        /// <param name="listener"></param>
        public void SubscribeListener(IDXFeedConnectionListener listener)
        {
            lock (mListenersMutex)
                mListeners.AddLast(listener);
        }

        /// <summary>
        /// Unsubscribe listener
        /// </summary>
        /// <param name="listener"></param>
        public void UnsubscribeListener(IDXFeedConnectionListener listener)
        {
            lock (mListenersMutex)
                mListeners.Remove(listener);
        }

        /// <summary>
        /// Calls the action on each listener asynchronously
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task CallListenersAsync(Action<IDXFeedConnectionListener> action)
        {
            return Task.Run(() =>
            {
                IDXFeedConnectionListener[] listeners;

                lock (mListenersMutex)
                {
                    listeners = new IDXFeedConnectionListener[mListeners.Count];
                    mListeners.CopyTo(listeners, 0);
                }

                for (int i = 0; i < listeners.Length; i++)
                {
                    var listener = listeners[i];
                    action(listener);
                }
            });
        }

        public Task CallStatusChange(IDXFeedConnection connection)
            => CallListenersAsync((listener) => listener.OnStatusChanged(connection, connection.State));

        public Task CallException(IDXFeedConnection connection, Exception exception)
            => CallListenersAsync((listener) => listener.OnException(connection, exception));
    }
}
