//if you do not have correct certificate on client site, just use validation type: 'none' for instance of service.

FOR EX:
MessageService.MessageServiceClient srv = new MessageService.MessageServiceClient("wshttp");

srv.ClientCredentials.ServiceCertificate.Authentication.CertificateValidationMode =
System.ServiceModel.Security.X509CertificateValidationMode.None;