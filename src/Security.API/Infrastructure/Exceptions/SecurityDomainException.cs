using System;

namespace Sofisoft.Accounts.Security.API.Infrastructure.Exceptions
{
    public class SecurityDomainException : Exception
    {
        private string errorId;

        public string ErrorId => errorId;

        public SecurityDomainException()
        { }

        public SecurityDomainException(string message)
            : base(message)
        { }

        public SecurityDomainException(string message, string errorId)
            : base(message)
        { 
            this.errorId = errorId;
        }
    }
}