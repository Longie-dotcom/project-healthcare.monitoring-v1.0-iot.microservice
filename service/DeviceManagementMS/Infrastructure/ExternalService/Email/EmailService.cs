using Application.Interface.IService;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Service
{
    public class EmailService : IEmailService
    {
        #region Attributes
        private readonly string fromEmail;
        private readonly string displayName;
        private readonly string smtpHost;
        private readonly int smtpPort;
        private readonly string username;
        private readonly string password;
        private readonly bool enableSsl;
        #endregion

        #region Properties
        #endregion

        public EmailService(
            string fromEmail,
            string displayName,
            string smtpHost,
            int smtpPort,
            string username,
            string password,
            bool enableSsl)
        {
            this.fromEmail = fromEmail;
            this.displayName = displayName;
            this.smtpHost = smtpHost;
            this.smtpPort = smtpPort;
            this.username = username;
            this.password = password;
            this.enableSsl = enableSsl;
        }

        #region Methods
        public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
        {
            var message = new MailMessage();
            message.From = new MailAddress(fromEmail, displayName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = "Reset Your Password";
            message.IsBodyHtml = true;
            message.Body = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        background-color: #f4f4f7;
                        margin: 0; padding: 0;
                    }}
                    .container {{
                        width: 100%;
                        max-width: 600px;
                        margin: 40px auto;
                        background-color: #ffffff;
                        border-radius: 8px;
                        box-shadow: 0 4px 12px rgba(0,0,0,0.1);
                        padding: 30px;
                    }}
                    h2 {{
                        color: #333333;
                    }}
                    p {{
                        color: #555555;
                        line-height: 1.6;
                    }}
                    .button {{
                        display: inline-block;
                        padding: 12px 24px;
                        background-color: #4a90e2;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 6px;
                        font-weight: bold;
                        margin-top: 20px;
                    }}
                    .footer {{
                        margin-top: 30px;
                        font-size: 12px;
                        color: #999999;
                        text-align: center;
                    }}
                    @media only screen and (max-width: 600px) {{
                        .container {{
                            padding: 20px;
                        }}
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h2>Password Reset Request</h2>
                    <p>Hello,</p>
                    <p>We received a request to reset your password for your account.</p>
                    <p>Click the button below to set a new password. This link will expire in 1 hour.</p>
                    <a href='{resetLink}' class='button'>Reset Password</a>
                    <p class='footer'>If you did not request this, you can safely ignore this email.</p>
                </div>
            </body>
            </html>";

            using var smtp = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            await smtp.SendMailAsync(message);
        }
        #endregion
    }
}
