using System;
using BlazorPong.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BlazorPong.Data;
using BlazorPong.Shared;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorPong
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddTransient<HubConnectionBuilder>();
            //services.AddTransient<GameHub>();
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<ServerGameController>();
            services.AddSignalR();
            services.AddHostedService<Broadcaster>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider sp)
        {
            //TODO -oFBE: Utilizza lo standard qui sotto
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    //The default HSTS value is 30 days.You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            // NB: Per ora utilizzo sempre la pagina dell'eccezione specifica anche in produzione,
            // essendo un progetto in corso di sviluppo e comunque a scopo dimostrativo.
            app.UseDeveloperExceptionPage();
            app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapHub<GameHub>("/gamehub");
                endpoints.MapFallbackToPage("/_Host");
            });

            //GlobalContext.GlobalHubContext = sp.GetService<GameHub>();
            //GlobalContext.GlobalServerGameController = sp.GetService<ServerGameController>();
        }
    }
}
