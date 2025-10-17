using EVote360.Application.Abstractions.Services;
using EVote360.Application.DTOs.Request;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace EVote360.Infrastructure.Emails
{
    public sealed class SmtpEmailService : IEmailService
    {
        private readonly InMemoryEmailQueue _queue;
        public SmtpEmailService(InMemoryEmailQueue queue, IOptions<EVote360.Shared.Options.EmailSenderOptions> _)
        {
            _queue = queue;
        }
        public async Task SendAsync(VoteReceiptRequestDto message, string subject, string body, CancellationToken cancellationToken)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress("no-reply@evote360.local", "eVote360"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(new MailAddress(message.ToEmail, message.ToName));

            using (var client = new SmtpClient("smtp.yourprovider.com", 587))
            {
                client.Credentials = new NetworkCredential("username", "password");
                client.EnableSsl = true;

                await client.SendMailAsync(mailMessage, cancellationToken);
            }
        }
        public Task QueueAsync(VoteReceiptRequestDto message, CancellationToken ct = default)
            => _queue.WriteAsync(message, ct).AsTask();
    }
}
