using System;

namespace VBaseProject.Domain.Exceptions
{
    [Serializable]
    public class UnauthorizedUserException : BusinessException
    {
        public UnauthorizedUserException(string msg, string title) : base(msg, title)
        {

        }
    }
}
