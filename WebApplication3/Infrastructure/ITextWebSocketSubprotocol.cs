using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Lib.AspNetCore.WebSocketsCompression.Providers;

namespace WebApplication3.Infrastructure
{
    internal interface ITextWebSocketSubProtocol
    {

        string SubProtocol { get; }

        Task SendAsync(string message, WebSocket socket, IWebSocketCompressionProvider socketCompressionProvider,CancellationToken cancellationToken);

        string Read(string rawMessage);

    }
}