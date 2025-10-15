namespace EVote360.Application.Abstractions.Emails;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body, bool isHtml = false, CancellationToken ct = default);
}
