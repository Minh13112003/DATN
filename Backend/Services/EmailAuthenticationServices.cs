
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using System.Net;

namespace DoAnTotNghiep.Services
{
    public class EmailAuthenticationServices : IEmailAuthenticationServices
    {
        private readonly IConfiguration _configuration;
        public EmailAuthenticationServices(IConfiguration configuration)
        {
            _configuration = configuration;    
        }
        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var email = new MimeKit.MimeMessage();
            email.From.Add(new MimeKit.MailboxAddress(_configuration["EmailSettings:SenderName"], _configuration["EmailSettings:SenderEmail"]));
            email.To.Add(MimeKit.MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new MimeKit.BodyBuilder
            {
                HtmlBody = htmlBody
            };

            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(_configuration["EmailSettings:SmtpServer"], int.Parse(_configuration["EmailSettings:SmtpPort"]!), MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_configuration["EmailSettings:SenderEmail"], _configuration["EmailSettings:SenderPassword"]);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        
    }
}
