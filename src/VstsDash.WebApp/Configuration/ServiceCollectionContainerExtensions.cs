using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VstsDash.WebApp.Configuration
{
    public static class ServiceCollectionContainerExtensions
    {
        public static IContainer AddContainer(
            this IServiceCollection services,
            IConfigurationRoot configuration,
            IHostingEnvironment env)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (env == null) throw new ArgumentNullException(nameof(env));

            var builder = new ContainerBuilder();

            builder.Populate(services);

            var teamServicesConfiguration = configuration.GetSection("TeamServices");
            var appSettings = new AppSettings(teamServicesConfiguration);

            builder.RegisterInstance(env);
            builder.RegisterInstance(appSettings);

            builder.RegisterModule<RestApiContainerModule>();
            builder.RegisterModule<AppServicesContainerModule>();
            builder.RegisterModule<MapperContainerModule>();

            var container = builder.Build();
            return container;
        }
    }
}