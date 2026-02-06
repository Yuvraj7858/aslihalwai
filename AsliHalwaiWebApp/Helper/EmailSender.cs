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
                var smtp = new SmtpClient
                {
                    Host = _config["smtp:Host"],
                    Port = int.Parse(_config["smtp:Port"]),
                    EnableSsl = true,
                    Credentials = new NetworkCredential(
         _config["smtp:FromEmail"],
         _config["smtp:Password"]
     )
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(_config["smtp:FromEmail"]),
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
