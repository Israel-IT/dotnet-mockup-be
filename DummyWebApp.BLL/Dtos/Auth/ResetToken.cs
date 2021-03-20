namespace DummyWebApp.BLL.Dtos.Auth
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ResetToken
    {
        public ResetToken(DateTime expireTime, string code)
        {
            ExpireTime = expireTime;
            Code = code;
        }

        [Required]
        public DateTime ExpireTime { get; }

        [Required]
        public string Code { get; }
    }
}