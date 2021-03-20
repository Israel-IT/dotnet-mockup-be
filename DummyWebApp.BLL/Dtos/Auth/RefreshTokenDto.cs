namespace DummyWebApp.BLL.Dtos.Auth
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using ValidationConstants.Auth;

    public class RefreshTokenDto
    {
        public RefreshTokenDto(string refreshToken)
        {
            RefreshToken = refreshToken;
        }

        /// <summary>
        /// Gets JWT refresh token.
        /// </summary>
        [DataMember]
        [Required(ErrorMessage = RefreshTokenValidationConstants.RefreshTokenIsRequired)]
        public string RefreshToken { get; }
    }
}