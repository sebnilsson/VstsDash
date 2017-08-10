using System;
using System.Globalization;
using System.IO.Compression;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VstsDash.WebApp.Configuration;
using VstsDash.WebApp.IpRestriction;

namespace VstsDash.WebApp
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var cultureInfo = new CultureInfo("en-US");

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings.secrets.json", true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            Environment = env;
        }

        public IContainer ApplicationContainer { get; private set; }

        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var loggingConfiguration = Configuration.GetSection("Logging");

            loggerFactory.AddConsole(loggingConfiguration);

            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseIpRestrictions(Configuration);
            }

            app.UseResponseCompression();

            app.UseStaticFiles();

            app.UseMvc(ConfigureRoutes);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc()
                .AddJsonOptions(
                    options =>
                    {
                        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                        options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                    });

            services.AddRouting(routing => { routing.LowercaseUrls = true; });

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

            services.AddResponseCompression(
                options =>
                {
                    options.MimeTypes = new[]
                                        {
                                            // Default
                                            "text/plain", "text/css", "application/javascript", "text/html",
                                            "application/xml", "text/xml", "application/json", "text/json",

                                            // Custom
                                            "image/svg+xml"
                                        };
                    options.EnableForHttps = true;
                });

            if (!Environment.IsDevelopment()) services.AddIpRestrictions(Configuration);

            ApplicationContainer = services.AddContainer(Configuration, Environment);

            return new AutofacServiceProvider(ApplicationContainer);
        }

        private static void ConfigureRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(RouteNames.Empty, string.Empty);

            routes.MapRoute(
                RouteNames.Meta,
                "meta",
                new
                {
                    controller = "Home",
                    action = "Meta"
                });

            routes.MapRoute(RouteNames.Default, "{controller=Home}/{action=Index}/{id?}");
        }
    }
}