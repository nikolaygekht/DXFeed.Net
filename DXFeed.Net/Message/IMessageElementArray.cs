using System.Collections.Generic;

namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is an array of values
    /// </summary>
    public interface IMessageElementArray : IMessageElement, IEnumerable<IMessageElement>
    {
        /// <summary>
        /// Returns the number of the elements in the array
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets or sets a message element by the index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IMessageElement this[int index] { get; set; }

        /// <summary>
        /// Adds an element to the array
        /// </summary>
        /// <param name="element"></param>
        void Add(IMessageElement element);
    }
}
