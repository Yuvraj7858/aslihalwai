using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace AsliHalwaiWebApp.CommonCode
{
    public class EmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(toEmail))
                    return false;

                string host = _config["smtp:Host"] ?? "smtp.gmail.com";
                int port = 587;
                if (int.TryParse(_config["smtp:Port"], out int parsedPort))
                {
                    port = parsedPort;
                }

                string fromEmail = _config["smtp:FromEmail"] ?? "no-reply@aslihalwai.com";
                string password = _config["smtp:Password"] ?? "";

                var smtp = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(fromEmail, password)
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(toEmail);
                smtp.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
