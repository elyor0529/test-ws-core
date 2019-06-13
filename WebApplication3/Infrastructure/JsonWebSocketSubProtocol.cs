using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Lib.AspNetCore.WebSocketsCompression.Providers;
using Newtonsoft.Json;

namespace WebApplication3.Infrastructure
{
    internal class JsonWebSocketSubProtocol : TextWebSocketSubProtocolBase, ITextWebSocketSubProtocol
    {

        public string SubProtocol => "aspnetcore-ws.json";

        public override Task SendAsync(string message, WebSocket socket,IWebSocketCompressionProvider socketCompressionProvider, CancellationToken cancellationToken)
        {
            var jsonMessage = JsonConvert.SerializeObject(new {message, timestamp = DateTime.UtcNow});

            return base.SendAsync(jsonMessage, socket, socketCompressionProvider, cancellationToken);
        }

    }
}