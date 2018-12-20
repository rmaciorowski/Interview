using System.ServiceModel;

using System.Runtime.Serialization;
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;

namespace PkoMessageService
{
    public class MessageService : IMessageService
    {
        private IValidator validator;
        public MessageResponse Send(MessageRequest message)
        { 
            if (message == null)
                return new MessageResponse { ErrorMessage = "request cannot be null", ReturnCode = ReturnCode.ValidationError };

            message.FilterContacts();

            MessageResponse response = new MessageResponse();
            validator = new Validator(message);

            if (validator.Validate())
            {
                using (EmailMessage mail = new EmailMessage(message))
                {
                    try
                    {
                        mail.SendEmail();
                    }
                    catch (Exception e)
                    {
                        response.ErrorMessage = e.Message+": "+e.InnerException;
                        response.ReturnCode = ReturnCode.InternalError;
                    }
                }
                response.ReturnCode = response.ReturnCode != ReturnCode.InternalError ? response.ReturnCode = ReturnCode.Success : ReturnCode.InternalError;
            }
            else
            {
                response.ReturnCode = ReturnCode.ValidationError;
                response.ErrorMessage = "Invalid request:";
                foreach (string error in validator.GetErrorList())
                    response.ErrorMessage += " @" + error;
            }
            SaveSendResultToDb(message, response);

            return response;
        }

        private void SaveSendResultToDb(MessageRequest message, MessageResponse response)
        {
            using (Model.MessageService db = new Model.MessageService())
            {
                Model.MessageHistory msgHistory = new Model.MessageHistory
                {
                    EmailAddress = message.Recipient.Contacts[0].Value,
                    EmailBody = message.Message,
                    EmailSubject = message.Subject,
                    ErrorCode = response.ReturnCode != ReturnCode.Success ? (int?)response.ReturnCode.GetHashCode() : null,
                    ErrorMessage = response.ErrorMessage,
                    SentDateTime = DateTime.Now
                };
                db.Messages.Add(msgHistory);
                db.SaveChanges();
            }
        }
    }
}
