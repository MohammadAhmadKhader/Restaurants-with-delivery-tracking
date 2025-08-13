using MailKit.Net.Smtp;
using MassTransit;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Notifications.Config;
using Notifications.Messages;
using Shared.Utils;

namespace Notifications.Consumers;

public class EmailBatchConsumer(
    IOptions<EmailSettings> emailSettings,
    ILogger<EmailBatchConsumer> logger) : IConsumer<Batch<EmailMessage>>
{
    private readonly EmailSettings _emailSettings = emailSettings.Value;
    private readonly ILogger<EmailBatchConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<Batch<EmailMessage>> context)
    {
        var batch = context.Message;
        _logger.LogInformation("Processing email batch of {Count} messages...", batch.Length);

        using var client = new SmtpClient();
        var successful = 0;
        var failed = 0;

        try
        {
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, _emailSettings.UseSSL);
            if (_emailSettings.UseAuth || EnvironmentUtils.IsProduction())
            {
                await client.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);
            }

            foreach (var messageContext in batch)
            {
                try
                {
                    var email = messageContext.Message;
                    var emailMsg = new MimeMessage
                    {
                        Subject = email.Subject,
                        Body = new TextPart(TextFormat.Html) { Text = email.TextBody }
                    };

                    emailMsg.From.Add(new MailboxAddress("RMS", _emailSettings.FromEmail));
                    emailMsg.To.Add(new MailboxAddress("", email.ToEmail));

                    await client.SendAsync(emailMsg);
                    successful++;

                    _logger.LogDebug("Email sent to {ToEmail}", email.ToEmail);
                }
                catch (Exception ex)
                {
                    failed++;
                    _logger.LogError(ex, "Failed to send email to {ToEmail}",
                        messageContext.Message.ToEmail);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to establish SMTP connection");
            failed = batch.Length;
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true);
            }
        }

        _logger.LogInformation("Batch completed: {Successful} successful, {Failed} failed",
            successful, failed);
    }
}