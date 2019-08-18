using System;

namespace VBaseProject.Service.Exceptions
{
    [Serializable]
    public class UnauthorizedUserException : BusinessException
    {
        public string Email { get; private set; }

        public UnauthorizedUserException(string email) : base($"Unauthorized access to the user {email}")
        {
            Email = email;
            Title = "Unauthorized";
        }
    }
}
