namespace VBaseProject.Resources.Messages
{
    /// <summary>
    /// Update All Resoure Files to matain the globalization support
    /// </summary>
    public class MessagesKeys
    {
        public class ExceptionsTitles
        {
            public const string ValidationError = "ExceptionTitle.ValidationError";
            public const string AuthenticationError = "Authentication.Error";
        }

        public class UserMessages
        {
            public const string EmptyPasword = "User.EmptyPassword";
            public const string EmailExists = "User.EmailExists";
            public const string InvalidCredentials = "User.InvalidCredentials";
            public const string InvalidRefreshToken = "User.InvalidRefreshToken";
        }

        public class CustomerMessages
        {
            public const string NullEntity = "Customer.NullEntity";
            public const string InvalidName = "Customer.InvalidName";
        }
    }
}
