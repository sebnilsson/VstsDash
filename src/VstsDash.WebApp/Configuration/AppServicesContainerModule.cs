using System.Reflection;
using Autofac;
using VstsDash.AppServices;
using Module = Autofac.Module;

namespace VstsDash.WebApp.Configuration
{
    public class AppServicesContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var appServiceType = typeof(IAppService);
            var appServiceTypeAssembly = appServiceType.GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(appServiceTypeAssembly).InstancePerLifetimeScope();
        }
    }
}