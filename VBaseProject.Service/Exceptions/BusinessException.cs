using System;

namespace VBaseProject.Service.Exceptions
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
