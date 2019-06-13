using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WebApplication3.MiddleWares
{
    internal static class WebSocketConnectionsMiddlewareExtensions
    {
        public static IApplicationBuilder MapWebSocketConnections(this IApplicationBuilder app, PathString pathMatch,WebSocketConnectionsOptions options)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));

            return app.Map(pathMatch,branchedApp => branchedApp.UseMiddleware<WebSocketConnectionsMiddleWare>(options));
        }
    }
}