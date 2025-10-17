using EVote360.Application.Abstractions.Repositories;
using EVote360.Application.Abstractions.Services;
using EVote360.Application.Common.Ocr;
using EVote360.Infrastructure.Emails;
using EVote360.Infrastructure.Ocr;
using EVote360.Infrastructure.Persistence;
using EVote360.Infrastructure.Repositories;
using EVote360.Infrastructure.Repositories.Base;
using EVote360.Infrastructure.Security;
using EVote360.Shared.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EVote360.Application.Common.Security;


namespace EVote360.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.Configure<EmailSenderOptions>(config.GetSection("EmailSender"));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IElectionRepository, ElectionRepository>();
            services.AddScoped<ICitizenRepository, CitizenRepository>();
            services.AddScoped<IPartyRepository, PartyRepository>();
            services.AddScoped<IPartyAssignmentRepository, PartyAssignmentRepository>();
            services.AddScoped<IVoteRepository, VoteRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();

            services.AddSingleton<InMemoryEmailQueue>();
            services.AddHostedService<EmailBackgroundService>();
            services.AddScoped<
                EVote360.Application.Abstractions.Services.IEmailService,
                EVote360.Infrastructure.Emails.SmtpEmailService>();

            services.AddScoped<IOcrService, TesseractOcrMock>();
            services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();

            // Auth
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Cuenta/Acceder";
                    options.AccessDeniedPath = "/Cuenta/Denegado";
                });

            return services;
        }
    }
}
