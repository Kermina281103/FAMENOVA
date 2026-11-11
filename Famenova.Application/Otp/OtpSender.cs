using Famenova.Application.Interfaces;
using famenova.Domain.Enums;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Famenova.Application.Otp
{
    public class EmailOtpSender// : IOtpSender
    {
        //private readonly SmtpOptions _smtp;
        //public EmailOtpSender(IOptions<SmtpOptions> options) => _smtp = options.Value;

        //public async Task SendAsync(OtpChannel channel, string destination, string code, OtpPurpose purpose)
        //{
        //    if (channel != OtpChannel.Email) throw new NotSupportedException("Channel not supported.");

        //    var message = new MimeMessage();
        //    message.From.Add(new MailboxAddress(_smtp.FromName, _smtp.FromAddress));
        //    message.To.Add(MailboxAddress.Parse(destination));
        //    message.Subject = "Your Famenova verification code";
        //    message.Body = new TextPart("plain")
        //    {
        //        Text = $"Your code is {code}. It expires in 5 minutes. If you didn't request it, ignore this message."
        //    };

        //    using var client = new SmtpClient();
        //    await client.ConnectAsync(_smtp.Host, _smtp.Port, SecureSocketOptions.StartTls);
        //    await client.AuthenticateAsync(_smtp.Username, _smtp.Password);
        //    await client.SendAsync(message);
        //    await client.DisconnectAsync(true);
        //}
    }
}
