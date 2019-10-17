namespace Junjuria.Common.Dtos
{
    using System.Net.Mail;

    public class EmailSent
    {
        public MailMessage Mail { get; set; }
        public bool Normalised { get; set; }
    }

}