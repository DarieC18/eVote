using eVote360.Application.Abstractions.Services;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EVote360.Application
{
    public static class ServicesRegistration
    {
        public static void AddApplicationLayerIoc(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IPartyService, PartyService>();
            services.AddTransient<IPositionService, PositionService>();
            services.AddTransient<ICitizenService, CitizenService>();
            services.AddTransient<IElectionService, ElectionService>();
            services.AddTransient<IVotacionService, VotacionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAdminDashboardService, AdminDashboardService>();
        }
    }
}
