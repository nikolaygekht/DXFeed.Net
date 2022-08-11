using System.Collections.Generic;

namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is a dictionary of named values
    /// </summary>
    public interface IMessageElementObject : IMessageElement, IEnumerable<IMessageElementObjectProperty>
    {
        /// <summary>
        /// Gets or sets a property by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMessageElement this[string name] { get; set; }
        
        /// <summary>
        /// Checks whether the object has a property
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasProperty(string name);
    }
}
