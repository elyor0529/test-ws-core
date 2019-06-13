using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication3.Infrastructure;

namespace WebApplication3.Services
{
    internal class WebSocketConnectionsService : IWebSocketConnectionsService
    {
        
        #region Fields

        private readonly ConcurrentDictionary<Guid, WebSocketConnection> _connections = new ConcurrentDictionary<Guid, WebSocketConnection>();

        #endregion

        #region Methods

        public void AddConnection(WebSocketConnection connection)
        {
            _connections.TryAdd(connection.Id, connection);
        }

        public void RemoveConnection(Guid connectionId)
        {
            _connections.TryRemove(connectionId, out _);
        }

        public Task SendToAllAsync(string message, CancellationToken cancellationToken)
        {
            var connectionsTasks = _connections.Values.Select(connection => connection.SendAsync(message, cancellationToken));

            return Task.WhenAll(connectionsTasks);
        }

        #endregion

    }
}