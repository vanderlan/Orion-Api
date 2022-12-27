using System;

namespace VBaseProject.Domain.Exceptions
{
    [Serializable]
    public class ConflictException : BusinessException
    {
        public ConflictException(string message, string title) : base(message, title)
        {

        }
    }
}
