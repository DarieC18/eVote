using eVote360.Application.Abstractions.Services;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.Common.Security;
using EVote360.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.Pkcs;
using System.Reflection;

namespace EVote360.Application
{
    public static class ServicesRegistration
    {
        public static void AddApplicationLayerIoc(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IPartyService, PartyService>();
            services.AddTransient<IPositionService, PositionService>();
            services.AddTransient<ICitizenService, CitizenService>();
            services.AddTransient<IElectionService, ElectionService>();
            services.AddTransient<IVotacionService, VotacionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IAdminDashboardService, AdminDashboardService>();
            services.AddTransient<IUserSession, UserSessionService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
