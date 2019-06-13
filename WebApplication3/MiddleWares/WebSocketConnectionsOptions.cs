using System.Collections.Generic;
using WebApplication3.Infrastructure;

namespace WebApplication3.MiddleWares
{
    internal class WebSocketConnectionsOptions
    {

        public WebSocketConnectionsOptions()
        {
            ReceivePayloadBufferSize = 4 * 1024;
        }

        public HashSet<string> AllowedOrigins { get; set; }

        public IList<ITextWebSocketSubProtocol> SupportedSubProtocols { get; set; }

        public ITextWebSocketSubProtocol DefaultSubProtocol { get; set; }

        public int? SendSegmentSize { get; set; }

        public int ReceivePayloadBufferSize { get; set; }

    }
}