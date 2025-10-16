namespace EVote360.Application.Common.Notifications;
public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
}
