using System;
using System.Text;

namespace DXFeed.Net.Message
{
    /// <summary>
    /// An element of a message
    /// </summary>
    public interface IMessageElement
    {
        /// <summary>
        /// The type of the element
        /// </summary>
        MessageElementType ElementType { get; }
    }
}
