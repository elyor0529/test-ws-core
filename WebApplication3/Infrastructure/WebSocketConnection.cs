using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Lib.AspNetCore.WebSocketsCompression.Providers;

namespace WebApplication3.Infrastructure
{
    internal class WebSocketConnection
    {
        #region Constructor

        public WebSocketConnection(WebSocket socket, IWebSocketCompressionProvider socketCompressionProvider, ITextWebSocketSubProtocol textSubProtocol, int receivePayloadBufferSize)
        {
            _webSocket = socket ?? throw new ArgumentNullException(nameof(socket));
            _webSocketCompressionProvider = socketCompressionProvider ?? throw new ArgumentNullException(nameof(socketCompressionProvider));
            _textSubProtocol = textSubProtocol ?? throw new ArgumentNullException(nameof(textSubProtocol));
            _receivePayloadBufferSize = receivePayloadBufferSize;
        }

        #endregion

        #region Fields

        private readonly WebSocket _webSocket;
        private readonly IWebSocketCompressionProvider _webSocketCompressionProvider;
        private readonly ITextWebSocketSubProtocol _textSubProtocol;
        private readonly int _receivePayloadBufferSize;

        #endregion

        #region Properties

        public Guid Id { get; } = Guid.NewGuid();
        public WebSocketCloseStatus? CloseStatus { get; private set; }
        public string CloseStatusDescription { get; private set; }

        #endregion

        #region Events

        public event EventHandler<string> ReceiveText;
        public event EventHandler<byte[]> ReceiveBinary;

        #endregion

        #region Methods

        public Task SendAsync(string message, CancellationToken cancellationToken)
        {
            return _textSubProtocol.SendAsync(message, _webSocket, _webSocketCompressionProvider, cancellationToken);
        }

        public Task SendAsync(byte[] message, CancellationToken cancellationToken)
        {
            return _webSocketCompressionProvider.CompressBinaryMessageAsync(_webSocket, message, cancellationToken);
        }

        public async Task ReceiveMessagesUntilCloseAsync()
        {
            try
            {
                var receivePayloadBuffer = new byte[_receivePayloadBufferSize];
                var webSocketReceiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(receivePayloadBuffer), CancellationToken.None);

                while (webSocketReceiveResult.MessageType != WebSocketMessageType.Close)
                {
                    if (webSocketReceiveResult.MessageType == WebSocketMessageType.Binary)
                    {
                        var webSocketMessage = await _webSocketCompressionProvider.DecompressBinaryMessageAsync(_webSocket, webSocketReceiveResult, receivePayloadBuffer);

                        OnReceiveBinary(webSocketMessage);
                    }
                    else
                    {
                        var webSocketMessage = await _webSocketCompressionProvider.DecompressTextMessageAsync(_webSocket, webSocketReceiveResult, receivePayloadBuffer);

                        OnReceiveText(webSocketMessage);
                    }

                    webSocketReceiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(receivePayloadBuffer), CancellationToken.None);

                }

                if (webSocketReceiveResult.CloseStatus != null)
                    CloseStatus = webSocketReceiveResult.CloseStatus.Value;

                CloseStatusDescription = webSocketReceiveResult.CloseStatusDescription;
            }
            catch (WebSocketException wsex) when (wsex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
            {
            }
        }

        private void OnReceiveText(string socketMessage)
        {
            var message = _textSubProtocol.Read(socketMessage);

            ReceiveText?.Invoke(this, message);
        }

        private void OnReceiveBinary(byte[] socketMessage)
        {
            ReceiveBinary?.Invoke(this, socketMessage);
        }

        #endregion
    }
}