namespace DummyWebApp.Controllers
{
    using System.ComponentModel.DataAnnotations;
    using System.Threading;
    using System.Threading.Tasks;
    using BLL.Dtos.Auth;
    using BLL.Services.Abstraction;
    using Extensions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
            => _authService = authService;

        /// <summary>
        /// Verifies code for resetting password.
        /// </summary>
        /// <param name="verifyResetPasswordToken">Verify reset password model.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Http result.</returns>
        [HttpPost("verify-reset-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyResetPasswordTokenAsync(
            VerifyResetPasswordTokenDto verifyResetPasswordToken,
            CancellationToken cancellationToken)
            => (await _authService.VerifyResetPasswordTokenAsync(verifyResetPasswordToken.Code, verifyResetPasswordToken.Email, cancellationToken)).ToActionResult();

        /// <summary>
        /// Creates new user.
        /// </summary>
        /// <param name="registerUserDto">User for creating.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Http result.</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterUserDto registerUserDto, CancellationToken cancellationToken)
            => (await _authService.RegisterUserAsync(registerUserDto, cancellationToken)).ToActionResult();

        /// <summary>
        /// Generates JWT access token.
        /// </summary>
        /// <param name="loginUserDto">Login params.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>JWT access token.</returns>
        [HttpPost("token")]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTokenAsync(LoginUserDto loginUserDto, CancellationToken cancellationToken)
            => (await _authService.GetTokenAsync(loginUserDto, cancellationToken)).ToActionResult();

        /// <summary>
        /// Refresh access token and returns new token and refresh token.
        /// </summary>
        /// <param name="refreshToken">Refresh token.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Access token and returns new token and refresh token.</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(TokenDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenDto refreshToken, CancellationToken cancellationToken)
            => (await _authService.RefreshTokenAsync(refreshToken.RefreshToken)).ToActionResult();

        /// <summary>
        /// Sends code for resetting password on email.
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Http result.</returns>
        [HttpPost("send-reset-token/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SendPasswordResetTokenAsync(
            [EmailAddress] string email,
            CancellationToken cancellationToken)
            => (await _authService.SendPasswordResetTokenAsync(email, cancellationToken)).ToActionResult();

        /// <summary>
        /// Sets new password by received code from email.
        /// </summary>
        /// <param name="resetPasswordDto">Reset password params.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Http result.</returns>
        [HttpPut("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken)
            => (await _authService.ResetPasswordAsync(resetPasswordDto, cancellationToken)).ToActionResult();
    }
}