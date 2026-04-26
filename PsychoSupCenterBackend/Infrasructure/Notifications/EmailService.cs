using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PsychoSupCenterBackend.Application.Common.Interfaces;

namespace Infrasructure.Notifications;

internal sealed class EmailService(IOptions<EmailSettings> emailOptions) : IEmailService
{
    private readonly EmailSettings _settings = emailOptions.Value;

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password, cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}