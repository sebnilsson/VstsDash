using System.Reflection;
using Autofac;
using VstsDash.RestApi;
using VstsDash.RestApi.Caching;
using Module = Autofac.Module;

namespace VstsDash.WebApp.Configuration
{
    public class RestApiContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Cache>().As<ICache>().SingleInstance();
            builder.RegisterType<RestApiClient>().As<IRestApiClient>().InstancePerLifetimeScope();
            builder.RegisterType<RestHttpClient>().As<IRestHttpClient>().InstancePerLifetimeScope();

            var apiServiceType = typeof(IApiService);

            var restApiServiceTypeAssembly = typeof(RestApiClient).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(restApiServiceTypeAssembly)
                .Where(x => apiServiceType.IsAssignableFrom(x))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}