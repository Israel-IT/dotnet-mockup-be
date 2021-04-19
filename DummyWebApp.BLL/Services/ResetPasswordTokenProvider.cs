namespace DummyWebApp.BLL.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Abstraction;
    using Core.ResultConstants;
    using Dtos.Auth;
    using ResultModel.Abstraction.Generics;
    using ResultModel.Generics;

    public class ResetPasswordTokenProvider : IResetPasswordTokenProvider
    {
        private readonly ConcurrentDictionary<int, ResetToken> _resetTokens;
        private readonly TimeSpan _tokenExpirationTime = TimeSpan.FromMinutes(1);

        public ResetPasswordTokenProvider()
        {
            _resetTokens = new ConcurrentDictionary<int, ResetToken>();
        }

        public ResetToken GenerateResetToken(int userId)
            => _resetTokens[userId] = new ResetToken(DateTime.UtcNow, GetResetCode(5));

        public IResult<bool> VerifyResetToken(string token, int userId)
        {
            if (!_resetTokens.TryGetValue(userId, out var resetToken))
            {
                return Result<bool>.CreateFailed(AuthServiceResultConstants.InvalidResetPasswordToken);
            }

            if (DateTime.UtcNow.Subtract(resetToken.ExpireTime) >= _tokenExpirationTime)
                return Result<bool>.CreateFailed(AuthServiceResultConstants.ExpiredResetPasswordToken);

            if (resetToken.Code != token)
                return Result<bool>.CreateFailed(AuthServiceResultConstants.InvalidResetPasswordToken);

            return Result<bool>.CreateSuccess(true);
        }

        private string GetResetCode(int size)
            => new(Enumerable
                .Range(0, size)
                .Select(_ => char.Parse(new Random().Next(0, 9).ToString()))
                .ToArray());
    }
}