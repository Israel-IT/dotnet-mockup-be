namespace DummyWebApp.BLL.Services.Abstraction
{
    using Dtos.Auth;
    using ResultModel.Abstraction.Generics;

    public interface IResetPasswordTokenProvider
    {
        ResetToken GenerateResetToken(int userId);

        IResult<bool> VerifyResetToken(string token, int userId);
    }
}