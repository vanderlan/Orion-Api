using System;

namespace VBaseProject.Domain.Exceptions
{
    [Serializable]
    public class BusinessException : Exception
    {
        public string Title { get; set; }

        public BusinessException(string message) : base(message)
        {

        }
        public BusinessException(string message, string title) : base(message)
        {
            Title = title;
        }
    }
}
