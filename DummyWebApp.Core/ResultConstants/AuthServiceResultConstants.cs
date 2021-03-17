namespace DummyWebApp.Core.ResultConstants
{
    public static class AuthServiceResultConstants
    {
        /// <summary>
        /// Represents message when user with the same email already exists in system.
        /// </summary>
        public const string UserAlreadyExists = "USER_EXISTS";

        /// <summary>
        /// Represents message when user with this email does not exist.
        /// </summary>
        public const string UserNotFound = "USER_NOT_FOUND";

        /// <summary>
        /// Represents message when user sent invalid username or password.
        /// </summary>
        public const string InvalidUserNameOrPassword = "IVALID_USERNAME_OR_PASSWORD";

        /// <summary>
        /// Represents message when user sent invalid code for password resetting.
        /// </summary>
        public const string InvalidResetPasswordToken = "IVALID_RESET_PASSWORD_TOKEN";

        /// <summary>
        /// Represents message when refresh token is not valid.
        /// </summary>
        public const string InvalidRefreshToken = "INVALID_REFRESH_TOKEN";

        /// <summary>
        /// Represents message when user sent valid code for resetting, but it is already expired.
        /// </summary>
        public const string ExpiredResetPasswordToken = "EXPIRED_RESET_TOKEN";
    }
}