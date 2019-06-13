using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication2
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebSockets(options =>
            {
                options.KeepAliveInterval = TimeSpan.FromMinutes(10);
                options.ReceiveBufferSize = 4 * 1024;
                options.AllowedOrigins.Add("http://localhost:5000");
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWebSockets();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.Map("/ws", SocketHandler.Map);

        }
    }
}
