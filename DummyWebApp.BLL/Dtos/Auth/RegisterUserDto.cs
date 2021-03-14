namespace DummyWebApp.BLL.Dtos.Auth
{
    using System.ComponentModel.DataAnnotations;
    using ValidationAttributes;

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
        [EmailAddress]
        [Required]
        public string Email { get; }

        /// <summary>
        /// Gets user password.
        /// </summary>
        [Password]
        [Required]
        public string Password { get; }
    }
}