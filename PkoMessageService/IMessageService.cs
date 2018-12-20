using System.Runtime.Serialization;
using System.ServiceModel;
using System.Linq;
using System.Security.Permissions;
using System.IdentityModel.Selectors;
using System.Configuration;

namespace PkoMessageService
{
    [ServiceContract]
    public interface IMessageService
    {
        [PrincipalPermission(SecurityAction.Demand,Role ="Administrator")]
        [OperationContract]
        MessageResponse Send(MessageRequest message);   
    }

    public class CustomUserNamePasswordValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if (userName != ConfigurationManager.AppSettings["serviceUser"] || password != ConfigurationManager.AppSettings["servicePswd"])
                throw new FaultException("wrong service credentials");
        }
    }

    [DataContract]
    public class MessageRequest
    {
        [DataMember]
        public string Subject { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public Recipient Recipient { get; set; }

        public void FilterContacts()
        {
            this.Recipient.Contacts = this.Recipient.Contacts
                .Where(c => c.ContactType == (this.Recipient.LegalForm == LegalForm.Company ? ContactType.OfficeEmail : ContactType.Email))
                .ToArray();
        }
    }

    [DataContract]
    public class Recipient
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public LegalForm LegalForm { get; set; }
        [DataMember]
        public Contact[] Contacts { get; set; }
    }

    [DataContract]
    public class Contact
    {
        [DataMember]
        public ContactType ContactType { get; set; }

        [DataMember]
        public string Value { get; set; }
    }

    [DataContract]
    public class MessageResponse
    {
        [DataMember]
        public ReturnCode ReturnCode { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
    }

    [DataContract]
    public enum ReturnCode
    {
        [EnumMember]
        Success,
        [EnumMember]
        ValidationError,
        [EnumMember]
        InternalError
    }

    [DataContract(Name ="LegalForm")]
    public enum LegalForm
    {
        [EnumMember]
        Person,

        [EnumMember]
        Company
    }

    [DataContract(Name ="ContactType")]
    public enum ContactType
    {
        [EnumMember]
        Mobile,

        [EnumMember]
        Fax,

        [EnumMember]
        Email,

        [EnumMember]
        OfficePhone,

        [EnumMember]
        OfficeFax,

        [EnumMember]
        OfficeEmail
    }
}
