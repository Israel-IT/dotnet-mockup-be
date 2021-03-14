namespace DummyWebApp.BLL.Services.Abstraction
{
    using BorsaLive.Core.Models.Abstraction.Generics;
    using Dtos.Auth;

    public interface IResetPasswordTokenProvider
    {
        ResetToken GenerateResetToken(int userId);

        IResult<bool> VerifyResetToken(string token, int userId);
    }
}