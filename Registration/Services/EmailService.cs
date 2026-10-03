using System.Net;
using System.Net.Mail;

namespace Registration.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmailAsync(
            string toEmail,
            string resetLink)
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var port = int.Parse(
                _configuration["EmailSettings:Port"] ?? "587");

            var senderName =
                _configuration["EmailSettings:SenderName"];

            var senderEmail =
                _configuration["EmailSettings:SenderEmail"];

            var username =
                _configuration["EmailSettings:Username"];

            var password =
                _configuration["EmailSettings:Password"];

            var enableSsl =
                bool.Parse(
                    _configuration["EmailSettings:EnableSsl"] ?? "true");

            using var client = new SmtpClient(smtpServer, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(
                    username,
                    password)
            };

            using var mail = new MailMessage
            {
                From = new MailAddress(
                    senderEmail!,
                    senderName),

                Subject = "Password Reset - Student Management System",

                Body = $"""
                       <h2>Password Reset</h2>

                       <p>Hello,</p>

                       <p>
                           We received a request to reset your password.
                       </p>

                       <p>
                           Click the button below to create a new password:
                       </p>

                       <p>
                           <a href="{resetLink}"
                              style="
                              display:inline-block;
                              padding:10px 20px;
                              background:#2563eb;
                              color:white;
                              text-decoration:none;
                              border-radius:5px;">
                              Reset Password
                           </a>
                       </p>

                       <p>
                           This link will expire in 30 minutes.
                       </p>

                       <p>
                           If you did not request this, you can safely
                           ignore this email.
                       </p>

                       <p>
                           Regards,<br/>
                           Student Management System
                       </p>
                       """,

                IsBodyHtml = true
            };

            mail.To.Add(toEmail);

            await client.SendMailAsync(mail);
        }
    }
}