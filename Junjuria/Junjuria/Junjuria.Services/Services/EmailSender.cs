namespace Junjuria.Services.Services
{
    using Abp.Net.Mail;
    using Junjuria.Common;
    using Microsoft.Extensions.Configuration;
    using MimeKit;
    using System.IO;
    using System.Net.Mail;
    using System.Threading.Tasks;

    public class EmailSender : IEmailSender
    {
        private MimeMessage message;
        private IConfigurationRoot configuration;
        public EmailSender()
        {
            message = new MimeMessage();
            message.From.Add(new MailboxAddress(GlobalConstants.JunjuriaEmailSenderName, GlobalConstants.JunjuriaEmail));
            configuration = new ConfigurationBuilder()
                                                    .SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("services-settings.json", optional: false, reloadOnChange: true)
                                                    .Build();
        }

        public void Send(string to, string subject, string body, bool isBodyHtml = true)
        {
            message.To.Add(new MailboxAddress(to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };
            flush(message);
        }

        private void flush(MimeMessage message)
        {
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                //// For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect("smtp-relay.sendinblue.com", 587, false);

                // Note: only needed if the SMTP server requires authentication
                var userName = configuration["SMTPSettings:Login"];
                var password = configuration["SMTPSettings:Password"];
                client.Authenticate(userName, password);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        public void Send(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            message.From.Clear();
            message.From.Add(new MailboxAddress(from, GlobalConstants.JunjuriaEmail));
            Send(to, subject, body, isBodyHtml);
        }

        public void Send(System.Net.Mail.MailMessage mail, bool normalize = true)
        {
            foreach (var address in mail.To)
            {
            Send(mail.From.DisplayName, address.Address, mail.Subject, mail.Body, mail.IsBodyHtml);
            }
        }

        public Task SendAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            return new Task(() => Send(to, subject, body, isBodyHtml));
        }

        public Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            return new Task(() => Send(from, to, subject, body, isBodyHtml));
        }

        public Task SendAsync(MailMessage mail, bool normalize = true)
        {
            return new Task(() => Send(mail,normalize));
        }
    }
}
