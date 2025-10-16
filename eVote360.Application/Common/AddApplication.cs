using AutoMapper;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EVote360.Application.Common;

public static class AddApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var profilesAssembly = Assembly.GetExecutingAssembly();

        var loggerFactory = LoggerFactory.Create(builder => { });

        var mapperConfig = new AutoMapper.MapperConfiguration(cfg =>
        {
            cfg.AddMaps(profilesAssembly);
        }, loggerFactory);

#if DEBUG
        mapperConfig.AssertConfigurationIsValid();
#endif

        services.AddSingleton<IMapper>(sp => mapperConfig.CreateMapper());

        services.AddScoped<IPositionService, PositionService>();
        services.AddScoped<IPartyService, PartyService>();


        return services;
    }
}
