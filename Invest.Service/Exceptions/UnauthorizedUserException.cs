using System;

namespace Invest.Service.Exceptions
{
    [Serializable]
    public class UnauthorizedUserException : BusinessException
    {
        public string Email { get; private set; }

        public UnauthorizedUserException()
        {
        }

        public UnauthorizedUserException(string email) : base(email)
        {
            Email = email;
        }

        public UnauthorizedUserException(string message, string email) : base(message)
        {
            Email = email;
        }
    }
}
