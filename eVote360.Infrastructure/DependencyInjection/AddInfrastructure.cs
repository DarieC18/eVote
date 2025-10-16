using EVote360.Application.Abstractions.Repositories;
using EVote360.Application.Common.Notifications;
using EVote360.Application.Common.Ocr;
using EVote360.Application.Common.Security;
using EVote360.Infrastructure.Notifications;
using EVote360.Infrastructure.Ocr;
using EVote360.Infrastructure.Persistence;
using EVote360.Infrastructure.Repositories;
using EVote360.Infrastructure.Repositories.Base;
using EVote360.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EVote360.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IElectionRepository, ElectionRepository>();
            services.AddScoped<ICitizenRepository, CitizenRepository>();
            services.AddScoped<IPartyRepository, PartyRepository>();
            services.AddScoped<IPartyAssignmentRepository, PartyAssignmentRepository>();
            services.AddScoped<IVoteRepository, VoteRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IOcrService, TesseractOcrMock>();
            services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.LoginPath = "/Cuenta/Acceder";
                    o.AccessDeniedPath = "/Cuenta/Denegado";
                    o.SlidingExpiration = true;
                });

            return services;
        }
    }
}
