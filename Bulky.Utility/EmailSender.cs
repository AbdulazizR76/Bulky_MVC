using BulkyBook.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;


namespace BulkyBook.Utility
{
    public class EmailSender : IEmailSender, IEmailService2
    {
        public string SendGridSecret { get; set; }
        private readonly EmailSettings _emailSettings;

        public EmailSender(IConfiguration _config,IOptions<EmailSettings> emailSettings)
        {
            SendGridSecret = _config.GetValue<string>("SendGrid:SecretKey");
            _emailSettings = emailSettings.Value;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //logic for Send Email 

            var client = new SendGridClient(SendGridSecret);

            var from = new EmailAddress("az.dev.76@gmail.com", "Bulky Book");
            var to = new EmailAddress(email);
            var message = MailHelper.CreateSingleEmail(from, to , subject, "", htmlMessage);

            return client.SendEmailAsync(message);
        }

        public async Task SendEmailAsync2(string toEmail, string subject, string htmlMessage)
        {
            var mail = new MailMessage()
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };

            mail.To.Add(toEmail);

            using var smtp = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                EnableSsl = true,
            };


            await smtp.SendMailAsync(mail);

            //return;
        }
    }
}
