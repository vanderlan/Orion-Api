using System;

namespace Orion.Domain.Exceptions
{
    [Serializable]
    public class UnauthorizedUserException : BusinessException
    {
        public UnauthorizedUserException(string message, string title) : base(message, title)
        {

        }
    }
}
