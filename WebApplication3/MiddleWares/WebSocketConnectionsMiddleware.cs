using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lib.AspNetCore.WebSocketsCompression;
using Microsoft.AspNetCore.Http;
using WebApplication3.Infrastructure;
using WebApplication3.Services;

namespace WebApplication3.MiddleWares
{
    internal class WebSocketConnectionsMiddleWare
    {
        #region Constructor

        public WebSocketConnectionsMiddleWare(RequestDelegate next, WebSocketConnectionsOptions options,IWebSocketConnectionsService connectionsService, IWebSocketCompressionService compressionService)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _connectionsService = connectionsService ?? throw new ArgumentNullException(nameof(connectionsService));
            _compressionService = compressionService ?? throw new ArgumentNullException(nameof(compressionService));
        }

        #endregion

        #region Fields

        private readonly WebSocketConnectionsOptions _options;
        private readonly IWebSocketConnectionsService _connectionsService;
        private readonly IWebSocketCompressionService _compressionService;

        #endregion

        #region Methods

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                if (ValidateOrigin(context))
                {
                    var textSubProtocol = NegotiateSubProtocol(context.WebSockets.WebSocketRequestedProtocols);
                    var webSocketCompressionProvider = _compressionService.NegotiateCompression(context);
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync(textSubProtocol?.SubProtocol);
                    var webSocketConnection = new WebSocketConnection(webSocket, webSocketCompressionProvider,textSubProtocol ?? _options.DefaultSubProtocol, _options.ReceivePayloadBufferSize);

                    webSocketConnection.ReceiveText += async (sender, message) =>
                    {
                        await webSocketConnection.SendAsync(message, CancellationToken.None);
                    };

                    _connectionsService.AddConnection(webSocketConnection);

                    await webSocketConnection.ReceiveMessagesUntilCloseAsync();

                    if (webSocketConnection.CloseStatus.HasValue)
                        await webSocket.CloseAsync(webSocketConnection.CloseStatus.Value,webSocketConnection.CloseStatusDescription, CancellationToken.None);

                    _connectionsService.RemoveConnection(webSocketConnection.Id);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                }
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private bool ValidateOrigin(HttpContext context)
        {
            return _options.AllowedOrigins == null || _options.AllowedOrigins.Count == 0 ||_options.AllowedOrigins.Contains(context.Request.Headers["Origin"].ToString());
        }

        private ITextWebSocketSubProtocol NegotiateSubProtocol(IList<string> requestedSubProtocols)
        {
            return _options.SupportedSubProtocols.FirstOrDefault(supportedSubProtocol => requestedSubProtocols.Contains(supportedSubProtocol.SubProtocol));
        }

        #endregion
    }
}