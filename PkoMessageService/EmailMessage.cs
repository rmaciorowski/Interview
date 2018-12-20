using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.Win32.SafeHandles;

namespace PkoMessageService
{
    public class EmailMessage : IEmailMessage
    {
        private bool isBodyHtml;
        private MessageRequest request;
        private SmtpSettings smtpSettings;
        private bool disposed = false;
        private SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        private EmailMessage() { }

        public EmailMessage(MessageRequest request)
        {
            this.request = request;
            isBodyHtml = IsMessageHtmlDoc(request.Message);
            GetSmtpSettings();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
                handle.Dispose();
            disposed = true;
        }

        private bool IsMessageHtmlDoc(string body)
        {
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(body);
            return !html.DocumentNode.Descendants().All(n => n.NodeType == HtmlNodeType.Text);
        }

        public void SendEmail()
        {
            using (SmtpClient client = new SmtpClient(smtpSettings.server))
            {
                client.UseDefaultCredentials = false;
                client.Port = smtpSettings.port;
                client.Credentials = new NetworkCredential(smtpSettings.user, smtpSettings.password);
                client.EnableSsl = smtpSettings.enableSsl;
                try
                {
                    client.Send(CreateMailMessage());
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e.InnerException);
                }
            }
        }

        private void GetSmtpSettings()
        {
            SmtpSettings settings = new SmtpSettings();

            if(ConfigurationManager.AppSettings["smtpSettingsFromAppConfig"] == "1")
            {
                settings.from = ConfigurationManager.AppSettings["smtpFrom"];
                settings.password = ConfigurationManager.AppSettings["smtpPassword"];
                settings.server = ConfigurationManager.AppSettings["smtpHost"];
                settings.user = ConfigurationManager.AppSettings["smtpUser"];
                settings.port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);
                settings.enableSsl = ConfigurationManager.AppSettings["smtpTlsSslRequired"] == "1" ? true : false;

            }
            else
            {
                //TODO: Get smtp settings from DB / currently we do not have config table for this feature
            }

            smtpSettings=settings;
        }

        private MailMessage CreateMailMessage()
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(smtpSettings.from);
            mailMessage.To.Add(String.Join(",", request.Recipient.Contacts.Select(c => c.Value)));
            mailMessage.Body = request.Message;
            mailMessage.Subject = request.Subject;
            mailMessage.IsBodyHtml = isBodyHtml;

            return mailMessage;
        }
    }
}
