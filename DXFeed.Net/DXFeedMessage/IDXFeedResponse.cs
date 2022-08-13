namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Interface to a response
    /// </summary>
    public interface IDXFeedResponse
    {
        /// <summary>
        /// The type of the response
        /// </summary>
        DXFeedResponseType ResponseType { get; }

        /// <summary>
        /// Return this as an instance of concrete response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T As<T>() where T : DXFeedResponse => (T)this;
    }
}
