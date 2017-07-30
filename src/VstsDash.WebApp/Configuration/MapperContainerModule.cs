using Autofac;
using AutoMapper;

namespace VstsDash.WebApp.Configuration
{
    public class MapperContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(RegisterMapper).SingleInstance();
        }

        private static void ConfigureMapper(IMapperConfigurationExpression config, IComponentContext context)
        {
            context = context.Resolve<IComponentContext>();

            config.ConstructServicesUsing(context.Resolve);

            var appServicesToViewModelMappingProfile = new ViewModelsMappingProfile(context);

            config.AddProfile(appServicesToViewModelMappingProfile);
        }

        private static IMapper RegisterMapper(IComponentContext context)
        {
            var configuration = new MapperConfiguration(config => ConfigureMapper(config, context));

            configuration.AssertConfigurationIsValid();

            var mapper = configuration.CreateMapper();
            return mapper;
        }
    }
}