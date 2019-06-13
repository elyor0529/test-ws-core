using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Lib.AspNetCore.WebSocketsCompression.Providers;

namespace WebApplication3.Infrastructure
{
    internal abstract class TextWebSocketSubProtocolBase
    {
        public virtual Task SendAsync(string message, WebSocket socket,IWebSocketCompressionProvider socketCompressionProvider, CancellationToken cancellationToken)
        {
            return socketCompressionProvider.CompressTextMessageAsync(socket, message, cancellationToken);
        }

        public virtual string Read(string socketMessage)
        {
            return socketMessage;
        }
    }
}