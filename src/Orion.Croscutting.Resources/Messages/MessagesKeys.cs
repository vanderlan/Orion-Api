namespace Orion.Croscutting.Resources.Messages;

/// <summary>
/// Update all Resource Files to mantain the globalization support
/// </summary>
public static class MessagesKeys
{
    public static class ExceptionsTitles
    {
        public const string ValidationError = "ExceptionTitle.ValidationError";
        public const string AuthenticationError = "Authentication.Error";
    }

    public static class UserMessages
    {
        public const string NullEntity = "User.NullEntity";
        public const string EmptyName = "User.EmptyName";
        public const string EmptyPasword = "User.EmptyPassword";
        public const string EmptyEmail = "User.EmptyEmail";
        public const string EmailExists = "User.EmailExists";
        public const string InvalidCredentials = "User.InvalidCredentials";
        public const string InvalidRefreshToken = "User.InvalidRefreshToken";
    }
}
