namespace DummyWebApp.BLL.Dtos.Auth
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

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
        [Required]
        public string RefreshToken { get; }
    }
}