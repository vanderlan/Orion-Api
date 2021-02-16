using System;

namespace VBaseProject.Service.Exceptions
{
    [Serializable]
    public class ConflictException : BusinessException
    {
        public ConflictException(string message, string title) : base(message, title)
        {

        }
    }
}
