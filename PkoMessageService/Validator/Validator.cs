using System.Text.RegularExpressions;
using System.Configuration;
using System;
using System.Collections.Generic;

namespace PkoMessageService
{
    public class Validator : IValidator
    {
        private MessageRequest request;
        private Regex emailRegex;
        private List<string> errors = new List<string>();

        /// <summary>
        /// object validates request
        /// </summary>
        /// <param name="request">Request.</param>
        public Validator(MessageRequest request)
        {
            this.request = request;
            try
            {
                //Get regex from config file
                emailRegex = new Regex(ConfigurationManager.AppSettings["emailRegEx"]);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public List<string> GetErrorList()
        {
            return errors;
        }

        public bool Validate()
        {
            bool v1 = IsContactTypeValid();
            bool v2 = IsContactEmailValid();
            bool v3 = IsFirstAndLastNameValid();
            return v1 && v2 && v3;
        }

        private bool IsContactEmailValid()
        {
            bool isValid = true;
            if (request.Recipient.Contacts.Length > 0)
            {
                foreach (Contact contact in request.Recipient.Contacts)
                {
                    if (!emailRegex.Match(contact.Value).Success)
                    {
                        errors.Add("Invalid email address("+contact.Value+")");
                        isValid = false;
                    }
                }
            }
            return isValid;
        }

        private bool IsContactTypeValid()
        {
            if(request.Recipient.Contacts.Length == 0)
            {
                errors.Add("Invalid contact type");
                return false;
            }
            return true;
        }

        private bool IsFirstAndLastNameValid()
        {
            if (request.Recipient.LastName == null && request.Recipient.LegalForm == LegalForm.Company)
            {
                errors.Add("field 'LastName' is required");
                return false;
            }
            else if (request.Recipient.LegalForm == LegalForm.Person && (request.Recipient.FirstName == null || request.Recipient.LastName == null))
            {
                errors.Add("field 'FirstName' and 'LastName' are required");
                return false;
            }
            return true;
        }
    }
}
