namespace Junjuria.Services.Services
{
    using sib_api_v3_sdk.Api;
    using sib_api_v3_sdk.Client;
    using sib_api_v3_sdk.Model;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class EmailSender
    {
        private readonly SMTPApi apiInstance;

        public EmailSender(string apikey)
        {
            Configuration.Default.ApiKey.Add("api-key", apikey);
            this.apiInstance = new SMTPApi();
        }

        public async Task SendEmailAsync(string senderName, string senderEmail, string topic, string contentHTML, string recieverMail, string recieverName = "user")
        {
            var subject = topic;
            var htmlContent = contentHTML;
            var sender = new SendSmtpEmailSender(senderName, senderEmail);
            var to = new List<SendSmtpEmailTo> { new SendSmtpEmailTo(recieverMail, recieverName) };
            var email = new SendSmtpEmail(sender, to, null, null, htmlContent, null, subject);
            try
            {
                CreateSmtpEmail result = await apiInstance.SendTransacEmailAsync(email);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("ERROR SMTP" + ex.Message);
            }
        }
    }
}