using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;
using PkoMessageService;
using System.Security.Cryptography.X509Certificates;

namespace StartService
{
    class Program
    {
        static void Main(string[] args)
        {
            MessageService srv = new MessageService();
            ServiceHost host = new ServiceHost(srv,new Uri("http://localhost:8081"));
            WSHttpBinding binding = new WSHttpBinding();
            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;

            host.AddServiceEndpoint(typeof(IMessageService), binding, "");
            host.Description.Behaviors.Find<ServiceBehaviorAttribute>().InstanceContextMode = InstanceContextMode.Single;
            host.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, "localhost");
            host.Open();
            Console.WriteLine("Service up");
            Console.ReadLine();
            host.Close();
        }
    }
}
