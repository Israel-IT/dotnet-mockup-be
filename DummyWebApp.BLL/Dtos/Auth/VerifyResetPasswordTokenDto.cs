namespace DummyWebApp.BLL.Dtos.Auth
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    public class VerifyResetPasswordTokenDto
    {
        public VerifyResetPasswordTokenDto(string code, string email)
        {
            Code = code;
            Email = email;
        }

        [DataMember]
        [Required]
        public string Code { get; }

        [DataMember]
        [Required]
        [EmailAddress]
        public string Email { get; }
    }
}