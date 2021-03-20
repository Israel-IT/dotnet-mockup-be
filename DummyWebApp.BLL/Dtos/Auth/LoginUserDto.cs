namespace DummyWebApp.BLL.Dtos.Auth
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using ValidationAttributes;
    using ValidationConstants.Auth;

    public class LoginUserDto
    {
        public LoginUserDto(string email, string password)
        {
            Email = email;
            Password = password;
        }

        /// <summary>
        /// Gets user email.
        /// </summary>
        [Required(ErrorMessage = SharedValidationConstants.RequiredEmail)]
        [DataMember]
        public string Email { get; }

        /// <summary>
        /// Gets user password.
        /// </summary>
        [Password]
        [DataMember]
        [Required(ErrorMessage = SharedValidationConstants.RequiredPassword)]
        public string Password { get; }
    }
}