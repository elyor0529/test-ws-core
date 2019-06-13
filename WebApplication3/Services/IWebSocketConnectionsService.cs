using System;
using System.Threading;
using System.Threading.Tasks;
using WebApplication3.Infrastructure;

namespace WebApplication3.Services
{
    internal interface IWebSocketConnectionsService
    {

        void AddConnection(WebSocketConnection connection);

        void RemoveConnection(Guid connectionId);

        Task SendToAllAsync(string message, CancellationToken cancellationToken);

    }
}
