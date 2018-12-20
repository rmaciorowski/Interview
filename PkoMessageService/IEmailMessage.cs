using System;

namespace PkoMessageService
{
    public interface IEmailMessage: IDisposable
    {
        void SendEmail();
    }
}
