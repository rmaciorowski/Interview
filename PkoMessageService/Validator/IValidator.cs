using System.Collections.Generic;

namespace PkoMessageService
{
    public interface IValidator
    {
        bool Validate();
        List<string> GetErrorList();
    }
}
