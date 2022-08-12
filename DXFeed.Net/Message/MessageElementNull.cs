namespace DXFeed.Net.Message
{
    /// <summary>
    /// Null value
    /// </summary>
    public class MessageElementNull : MessageElement, IMessageElementNull
    {
        public MessageElementNull() : base(MessageElementType.Null)
        {
        }
    }
}
