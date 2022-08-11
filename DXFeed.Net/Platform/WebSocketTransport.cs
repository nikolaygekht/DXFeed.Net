using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace DXFeed.Net.Platform
{
    internal sealed class WebSocketTransport : ITransport
    {
        private readonly ClientWebSocket mClient;

        public Encoding Encoding { get; set; } = Encoding.ASCII;

        public TransportState State
        {
            get
            {
                return mClient.State switch
                {
                    WebSocketState.Open => TransportState.Open,
                    _ => TransportState.Closed,
                };
            }
        }

        public WebSocketTransport()
        {
            mClient = new ClientWebSocket();
        }

        public void Dispose()
        {
            mClient.Dispose();
        }

        public void Connect(string url)
        {
            var uri = new Uri(url);
            mClient.ConnectAsync(uri, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public void Send(string message)
        {
            mClient.SendAsync(Encoding.GetBytes(message), WebSocketMessageType.Text, true, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private readonly object mBufferMutex = new object();
        private readonly byte[] mBuffer = new byte[65536];

        public string? Receive()
        {
            lock (mBufferMutex)
            {
                var buffer = new ArraySegment<byte>(mBuffer);
                var result = mClient.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
                if (result.Count > 0)
                    return Encoding.GetString(mBuffer, buffer.Offset, result.Count);
                return null;
            }
        }

        public void Close()
        {
            mClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
        }

    }
}
