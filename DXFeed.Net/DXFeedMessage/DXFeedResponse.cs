namespace DXFeed.Net.DXFeedMessage
{
    /// <summary>
    /// Base class for all responses
    /// </summary>
    public abstract class DXFeedResponse : IDXFeedResponse
    {
        /// <summary>
        /// The type of the response
        /// </summary>
        public DXFeedResponseType ResponseType { get; }

        /// <summary>
        /// Return this as an instance of concrete response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>() where T : DXFeedResponse => (T)this;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="responseType"></param>
        protected DXFeedResponse(DXFeedResponseType responseType)
        {
            ResponseType = responseType;
        }   
    }
}
