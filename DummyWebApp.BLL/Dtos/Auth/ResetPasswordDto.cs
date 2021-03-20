namespace DummyWebApp.BLL.Dtos.Auth
{
    using System.ComponentModel.DataAnnotations;
    using ValidationAttributes;
    using ValidationConstants.Auth;

    public class ResetPasswordDto
    {
        public ResetPasswordDto(string email, string newPassword, string token)
        {
            Email = email;
            NewPassword = newPassword;
            Token = token;
        }

        /// <summary>
        /// Gets email of user.
        /// </summary>
        [Required(ErrorMessage = SharedValidationConstants.RequiredEmail)]
        [EmailAddress(ErrorMessage = SharedValidationConstants.InvalidEmail)]
        public string Email { get; }

        /// <summary>
        /// Gets new password for resetting.
        /// </summary>
        [Required(ErrorMessage = ResetPasswordValidationConstants.NewPasswordRequired)]
        [Password]
        public string NewPassword { get; }

        /// <summary>
        /// Gets reset password code.
        /// </summary>
        [Required(ErrorMessage = ResetPasswordValidationConstants.TokenRequired)]
        public string Token { get; }
    }
}