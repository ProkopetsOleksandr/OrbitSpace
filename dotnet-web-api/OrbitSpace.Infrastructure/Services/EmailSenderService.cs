using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using OrbitSpace.Application.Services.Interfaces;
using OrbitSpace.Infrastructure.Configuration;

namespace OrbitSpace.Infrastructure.Services;

public class EmailSenderService(IOptions<SmtpOptions> smtpOptions) : IEmailSenderService
{
    private readonly SmtpOptions _settings = smtpOptions.Value;

    public async Task SendAsync(string subject, string to, string htmlBody, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_settings.From));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart(TextFormat.Html) { Text = htmlBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.None, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}