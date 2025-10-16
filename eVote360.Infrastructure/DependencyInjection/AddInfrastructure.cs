using EVote360.Application.Abstractions.Repositories;
using EVote360.Application.Common.Notifications;
using EVote360.Application.Common.Ocr;
using EVote360.Application.Common.Security;
using EVote360.Application.Interfaces;
using EVote360.Application.Services;
using EVote360.Application.Votacion;
using EVote360.Infrastructure.Notifications;
using EVote360.Infrastructure.Ocr;
using EVote360.Infrastructure.Persistence;
using EVote360.Infrastructure.Repositories;
using EVote360.Infrastructure.Repositories.Base;
using EVote360.Infrastructure.Security;
using EVote360.Infrastructure.Votacion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EVote360.Application.Elecciones;
using EVote360.Infrastructure.Elecciones;
using EVote360.Application.Abstractions.Services;
using EVote360.Infrastructure.Users;

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
            services.AddScoped<IVotacionService, VotacionService>();
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IOcrService, TesseractOcrMock>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPasswordHasher,Pbkdf2PasswordHasher>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IElectionService, ElectionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserService, EVote360.Infrastructure.Users.UserService>();



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
