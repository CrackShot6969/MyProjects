using System;
using System.Linq;
using System.Net.Mail;
using VendingDataExtractor.Interfaces;

namespace VendingDataExtractor.Subscribers
{
    public class EmailNotifier : INotifier<string>
    {
        private readonly ISettingsReader _reader; 

        public EmailNotifier(ISettingsReader reader)
        {
            this._reader = reader;
        }

        public bool Notify(string message)
        {
            bool successful = false;
            if ((this._reader.GetContact() != String.Empty) && (this._reader.GetContactMessage() != String.Empty))
            {
                successful = SendEmail(message);
            }
            return successful;
        }

        private bool SendEmail(string message)
        {
            bool successful;
            try
            {
                var mailMessage = new MailMessage();
                _reader.GetContact().Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(i => mailMessage.To.Add(i));
                mailMessage.To.Add(_reader.GetContact());
                mailMessage.Subject = "!!IMPORTANT!!DATA EXTRACTION FAILURE!!";
                mailMessage.From = new MailAddress("vendingdataextractor@limitlesssupplements.co.nz");
                mailMessage.Body = _reader.GetContactMessage() + message;
                mailMessage.IsBodyHtml = true;
                var smtp = new SmtpClient("smtp.live.com")
                {
                    Port = 587,
                    UseDefaultCredentials = false,
                    Credentials =
                        new System.Net.NetworkCredential(
                            System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(_reader.GetAccountUsername())),
                            System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(_reader.GetAccountPassword()))),
                    EnableSsl = true
                };
                smtp.Send(mailMessage);
                successful = true;
            }
            catch
            {
                successful = false;
            }
            return successful;
        }
    }
}