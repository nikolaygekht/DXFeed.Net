using System.Collections;
using System.Collections.Generic;

namespace DXFeed.Net.Message
{
    /// <summary>
    /// The message element is a dictionary of named values
    /// </summary>
    public class MessageElementObject : MessageElement, IMessageElementObject
    {
        private readonly Dictionary<string, IMessageElementObjectProperty> mProperties = new Dictionary<string, IMessageElementObjectProperty>();

        /// <summary>
        /// Gets or sets a property by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMessageElement this[string name] 
        {
            get => mProperties[name].Value;
            set => mProperties[name] = new MessageElementObjectProperty(name, value);
        }

        /// <summary>
        /// Checks whether the object has a property
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasProperty(string name) => mProperties.ContainsKey(name);

        public IEnumerator<IMessageElementObjectProperty> GetEnumerator() => mProperties.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Constructor
        /// </summary>
        public MessageElementObject() : base(MessageElementType.Object)
        {
        }
    }
}
