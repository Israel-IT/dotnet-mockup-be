namespace DummyWebApp.BLL.Dtos.Auth
{
    using System;

    public class ResetToken
    {
        public ResetToken(DateTime expireTime, string code)
        {
            ExpireTime = expireTime;
            Code = code;
        }

        public DateTime ExpireTime { get; }

        public string Code { get; }
    }
}