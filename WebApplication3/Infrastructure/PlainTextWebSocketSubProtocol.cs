namespace WebApplication3.Infrastructure
{
    internal class PlainTextWebSocketSubProtocol : TextWebSocketSubProtocolBase, ITextWebSocketSubProtocol
    {
        public string SubProtocol => "aspnetcore-ws.plaintext";
    }
}
