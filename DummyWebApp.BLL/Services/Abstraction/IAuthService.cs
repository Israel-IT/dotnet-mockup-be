namespace DummyWebApp.BLL.Services.Abstraction
{
    using System.Threading;
    using System.Threading.Tasks;
    using Dtos.Auth;
    using ResultModel.Abstraction;
    using ResultModel.Abstraction.Generics;

    public interface IAuthService
    {
        Task<IResult> RegisterUserAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken = default);

        Task<IResult<TokenDto>> GetTokenAsync(LoginUserDto loginUserDto, CancellationToken cancellationToken = default);

        Task<IResult> SendPasswordResetTokenAsync(string email, CancellationToken cancellationToken = default);

        Task<IResult> VerifyResetPasswordTokenAsync(string token, string email, CancellationToken cancellationToken = default);

        Task<IResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken = default);

        Task<IResult<TokenDto>> RefreshTokenAsync(string refreshToken);
    }
}