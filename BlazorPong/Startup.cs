using BlazorPong.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorPong.Data;
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
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<GameHub>();
            services.AddSingleton<ServerGameController>();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
                // Gli faccio usare il gamehub
                endpoints.MapHub<GameHub>("/gamehub");
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
