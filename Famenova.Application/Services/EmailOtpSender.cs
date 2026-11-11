using Famenova.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famenova.Application.Otp;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
namespace Famenova.Application.Services
{
 

    public class EmailOtpSender : IOtpSender
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailOtpSender> _logger;

        public EmailOtpSender(IOptions<EmailSettings> options, ILogger<EmailOtpSender> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendAsync(string destination, string code)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(MailboxAddress.Parse(destination));
            message.Subject = "Your Famenova verification code";
            message.Body = new BodyBuilder
            {
                TextBody = $"Your verification code is {code}. It expires in 5 minutes.",
                HtmlBody = $"<p>Your verification code is:</p>" +
                           $"<p style=\"font-size:24px;letter-spacing:4px\"><strong>{code}</strong></p>" +
                           "<p>It expires in 5 minutes. If you did not request it, ignore this email.</p>"
            }.ToMessageBody();

            try
            {
                using var client = new SmtpClient { Timeout = 15_000 };

                var socket = _settings.Port == 465
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTls;

                await client.ConnectAsync(_settings.Server, _settings.Port, socket);
                await client.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email via {Server}:{Port}", _settings.Server, _settings.Port);
                throw;
            }
        }
    }
}
