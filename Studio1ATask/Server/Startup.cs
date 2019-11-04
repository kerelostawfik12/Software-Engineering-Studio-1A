using System;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Studio1BTask.Models;

namespace Studio1BTask
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "../Client/dist"; });

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<DbContext>()
                .BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "../Client";

                if (env.IsDevelopment())
                {
                    var existingPort = 4200;

                    // If an existing angular server is running, use that. (TODO: Make this work for different ports)
                    if (IsPortInUse(existingPort))
                    {
                        spa.UseProxyToSpaDevelopmentServer("http://localhost:" + existingPort);
                        Console.WriteLine("Trying to use existing angular server on port " + existingPort);
                    }
                    else
                    {
                        spa.UseAngularCliServer("start-arbitrary-port");
                        Console.WriteLine("Could not find angular server on port " + existingPort +
                                          " - starting one up now");
                    }
                }
            });
        }

        private static bool IsPortInUse(int port)

        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            var ipEndPoints = ipProperties.GetActiveTcpListeners();

            return ipEndPoints.Any(endPoint => endPoint.Port == port);
        }
    }
}