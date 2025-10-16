using EVote360.Application.Common.Notifications;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace EVote360.Infrastructure.Notifications;
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _cfg;
    public SmtpEmailService(IConfiguration cfg) => _cfg = cfg;

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        var host = _cfg["Smtp:Host"];
        var port = int.Parse(_cfg["Smtp:Port"] ?? "587");
        var user = _cfg["Smtp:User"];
        var pass = _cfg["Smtp:Pass"];
        var from = _cfg["Smtp:From"] ?? user ?? "no-reply@evote.local";

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(user, pass),
            EnableSsl = true
        };
        var mail = new MailMessage(from, to, subject, htmlBody) { IsBodyHtml = true };
        await client.SendMailAsync(mail);
    }
}
