namespace DummyWebApp.BLL.Dtos.Auth
{
    using System.ComponentModel.DataAnnotations;
    using ValidationAttributes;
    using ValidationConstants.Auth;

    public class RegisterUserDto
    {
        public RegisterUserDto(string email, string password)
        {
            Email = email;
            Password = password;
        }

        /// <summary>
        /// Gets user email.
        /// </summary>
        [EmailAddress(ErrorMessage = SharedValidationConstants.InvalidEmail)]
        [Required(ErrorMessage = SharedValidationConstants.RequiredEmail)]
        public string Email { get; }

        /// <summary>
        /// Gets user password.
        /// </summary>
        [Password]
        [Required(ErrorMessage = SharedValidationConstants.RequiredPassword)]
        public string Password { get; }
    }
}