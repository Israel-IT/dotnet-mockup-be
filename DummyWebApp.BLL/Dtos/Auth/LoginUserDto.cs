namespace DummyWebApp.BLL.Dtos.Auth
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using ValidationAttributes;

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
        [Required]
        [DataMember]
        public string Email { get; }

        /// <summary>
        /// Gets user password.
        /// </summary>
        [Password]
        [DataMember]
        [Required]
        public string Password { get; }
    }
}