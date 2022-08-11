using System.Collections;
using System.Collections.Generic;

namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is an array of values
    /// </summary>
    public class MessageElementArray : MessageElement, IMessageElementArray
    {
        private readonly List<IMessageElement> mElements = new List<IMessageElement>();

        /// <summary>
        /// Returns the number of the elements in the array
        /// </summary>
        public int Length => mElements.Count;

        /// <summary>
        /// Gets or sets a message element by the index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IMessageElement this[int index] 
        {
            get => mElements[index];
            set => mElements[index] = value;
        }

        /// <summary>
        /// Adds an element to the array
        /// </summary>
        /// <param name="element"></param>
        public void Add(IMessageElement element) => mElements.Add(element);

        public IEnumerator<IMessageElement> GetEnumerator() => mElements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public MessageElementArray() : base(MessageElementType.Array)
        {
        }
    }
}
