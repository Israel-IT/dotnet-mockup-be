namespace DummyWebApp.BLL.Dtos.Auth
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using ValidationConstants.Auth;

    public class VerifyResetPasswordTokenDto
    {
        public VerifyResetPasswordTokenDto(string code, string email)
        {
            Code = code;
            Email = email;
        }

        [DataMember]
        [Required(ErrorMessage = VerifyResetPasswordTokenValidationConstants.CodeRequired)]
        public string Code { get; }

        [DataMember]
        [Required(ErrorMessage = SharedValidationConstants.RequiredEmail)]
        [EmailAddress(ErrorMessage = SharedValidationConstants.InvalidEmail)]
        public string Email { get; }
    }
}