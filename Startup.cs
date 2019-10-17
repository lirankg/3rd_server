using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server.Models;
using Server.Services;

namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var settingsVersion = Configuration.GetSection("File:FileVersion");

            services.Configure<CustomConfiguration>(Configuration.GetSection("CustomConfig"));
            Environment.SetEnvironmentVariable("BuildDate", new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).LastWriteTime.ToString("s"));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;

            });

            services.AddSingleton<TsvHandler>();
            services.AddSingleton<InMemoryServiceHandler>();
            services.AddSingleton<InMemoryService>();
            services.AddSingleton<StreamHandler>();
            //services.AddTransient<TriggerHandler>();
            services.AddSingleton<ResponseHandler>();

            Newtonsoft.Json.JsonConvert.DefaultSettings = () => new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            };
            //services.AddCors(action =>
            //   action.AddPolicy("Any", builder =>
            //      c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());  

            //  .AllowAnyMethod()            
            //  .AllowAnyHeader()));
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
            });

            services.AddSingleton(services);
            services.AddMvc(options => options.EnableEndpointRouting = false).AddNewtonsoftJson().AddControllersAsServices().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseForwardedHeaders();
            app.UseWebSockets();

           
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseCors("Any");

            app.ApplicationServices.GetService<InMemoryService>();
            app.ApplicationServices.GetService<StreamHandler>();

            app.UseFileServer();

        }


    }
}
