namespace Orion.Crosscutting.Resources.Messages;

/// <summary>
/// Update all resource files to maintain globalization support
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
        //Auth
        public const string InvalidCredentials = "User.InvalidCredentials";
        public const string InvalidRefreshToken = "User.InvalidRefreshToken";

        //User
        public const string NullEntity = "User.NullEntity";
        public const string EmptyName = "User.EmptyName";
        public const string EmptyPassword = "User.EmptyPassword";
        public const string EmptyEmail = "User.EmptyEmail";
        public const string EmptyId = "User.EmptyId";
        public const string EmailExists = "User.EmailExists";
        public const string InvalidPassword = "User.InvalidPassword";

        public const string EmptyNewPassword = "User.InvalidNewPassword";
        public const string EmptyNewPasswordConfirmation = "User.EmptyNewPasswordConfirmation";
        public const string PasswordAndConfirmationDifferent = "User.PasswordConfirmationDifferent";
    }
}
