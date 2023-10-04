using AutoMapper;
using Orion.Api.AutoMapper.Config;

namespace Orion.Api.AutoMapper
{
    public static class AutoMapperConfiguration
    {
        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new InputToDomainProfile());
                mc.AddProfile(new DomainToOutputProfile());
            });

            services.AddScoped(_ => mappingConfig.CreateMapper());
        }
    }
}
