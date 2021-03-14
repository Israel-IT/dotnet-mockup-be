namespace DummyWebApp.BLL.Dtos.Auth
{
    using System.ComponentModel.DataAnnotations;
    using ValidationAttributes;

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
        [Required]
        [EmailAddress]
        public string Email { get; }

        /// <summary>
        /// Gets new password for resetting.
        /// </summary>
        [Required]
        [Password]
        public string NewPassword { get; }

        /// <summary>
        /// Gets reset password code.
        /// </summary>
        [Required]
        public string Token { get; }
    }
}