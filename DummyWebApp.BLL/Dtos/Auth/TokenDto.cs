namespace DummyWebApp.BLL.Dtos.Auth
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    public class TokenDto
    {
        public TokenDto(string token, DateTime tokenExpTime, string refreshToken, DateTime refreshTokenExpTime)
        {
            Token = token;
            TokenExpTime = tokenExpTime;
            RefreshToken = refreshToken;
            RefreshTokenExpTime = refreshTokenExpTime;
        }

        /// <summary>
        /// Gets JWT access token.
        /// </summary>
        [DataMember]
        [Required]
        public string Token { get; }

        /// <summary>
        /// Gets time when access token will be expired.
        /// </summary>
        [DataMember]
        [Required]
        public DateTime TokenExpTime { get; }

        /// <summary>
        /// Gets JWT refresh token.
        /// </summary>
        [DataMember]
        [Required]
        public string RefreshToken { get; }

        /// <summary>
        /// Gets time when refresh token will be expired.
        /// </summary>
        [DataMember]
        [Required]
        public DateTime RefreshTokenExpTime { get; }
    }
}