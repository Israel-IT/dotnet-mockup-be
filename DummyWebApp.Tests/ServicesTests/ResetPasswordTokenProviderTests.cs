namespace DummyWebApp.Tests.ServicesTests
{
    using System;
    using BLL.Services;
    using BLL.Services.Abstraction;
    using Xunit;

    public class ResetPasswordTokenProviderTests
    {
        private readonly IResetPasswordTokenProvider _resetPasswordTokenProvider;

        public ResetPasswordTokenProviderTests()
        {
            _resetPasswordTokenProvider = new ResetPasswordTokenProvider();
        }
        
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [Theory]
        public void ShouldGenerateValidResetToken(int userId)
        {
            var resetToken = _resetPasswordTokenProvider.GenerateResetToken(userId);
            
            Assert.NotNull(resetToken);
            Assert.NotNull(resetToken.Code);
            Assert.True(resetToken.ExpireTime <= DateTime.UtcNow);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ShouldValidateResetToken(int userId)
        {
            var resetToken = _resetPasswordTokenProvider.GenerateResetToken(userId);
            var validateResult = _resetPasswordTokenProvider.VerifyResetToken(resetToken.Code, userId);
            
            Assert.NotNull(validateResult);
            Assert.True(validateResult.Data);
            Assert.True(validateResult.Success);
        }
    }
}