using EVote360.Infrastructure.Emails;
using EVote360.Shared.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

public sealed class EmailBackgroundService : BackgroundService
{
    private readonly InMemoryEmailQueue _queue;
    private readonly EmailSenderOptions _opts;
    private readonly ILogger<EmailBackgroundService> _logger;
    private SmtpClient? _smtpClient;

    public EmailBackgroundService(
        InMemoryEmailQueue queue,
        IOptions<EmailSenderOptions> opts,
        ILogger<EmailBackgroundService> logger)
    {
        _queue = queue;
        _opts = opts.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _smtpClient = new SmtpClient(_opts.Host, _opts.Port)
        {
            EnableSsl = _opts.EnableSsl,
            Credentials = new NetworkCredential(_opts.User, _opts.Password)
        };

        await foreach (var msg in _queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var mail = new MailMessage
                {
                    From = new MailAddress(_opts.FromEmail, _opts.FromName),
                    Subject = msg.Subject,
                    Body = msg.HtmlBody,
                    IsBodyHtml = true
                };
                mail.To.Add(new MailAddress(msg.ToEmail, msg.ToName));

                await _smtpClient.SendMailAsync(mail, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo enviando email a {To}", msg.ToEmail);
            }
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_smtpClient != null)
        {
            _smtpClient.Dispose();
        }

        await base.StopAsync(cancellationToken);
    }
}
