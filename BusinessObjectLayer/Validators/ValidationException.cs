using System;

namespace BusinessObjectLayer.Validators
{
    public class ValidationException : ApplicationException
    {
        public ValidationException(string message): base(message)
        {
        }
    }
}
