namespace PkoMessageService
{
    public struct SmtpSettings
    {
        public string server;
        public string user;
        public string password;
        public string from;
        public int port;
        public bool enableSsl;
    }
}
