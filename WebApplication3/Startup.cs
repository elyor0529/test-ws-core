using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication3.Infrastructure;
using WebApplication3.MiddleWares;
using WebApplication3.Services;

namespace WebApplication3
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebSocketCompression();
            services.AddWebSocketConnections();
            services.AddSingleton<IHostedService, HeartbeatService>();
        }

        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSocketsCompression();
            app.MapWebSocketConnections("/socket", new WebSocketConnectionsOptions
            {
                AllowedOrigins = new HashSet<string>
                {
                    "http://localhost:5000"
                },
                SupportedSubProtocols = new List<ITextWebSocketSubProtocol>
                {
                    new JsonWebSocketSubProtocol(),
                    new PlainTextWebSocketSubProtocol()
                },
                DefaultSubProtocol = new PlainTextWebSocketSubProtocol(),
                SendSegmentSize = 8 * 1024
            });
            app.Run(async context =>
            {
                await context.Response.WriteAsync("-- Demo.AspNetCore.WebSocket --");
            });

        }
    }
}