using eVote360.Application.Abstractions.Services;
using eVote360.Application.ViewModels.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EVote360.Application.Common;

public static class AddApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IPositionService, PositionService>();
        return services;
    }
}
