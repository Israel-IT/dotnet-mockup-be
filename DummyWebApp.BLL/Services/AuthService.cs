namespace DummyWebApp.BLL.Services
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Abstraction;
    using BorsaLive.Core.Models;
    using BorsaLive.Core.Models.Abstraction;
    using BorsaLive.Core.Models.Abstraction.Generics;
    using BorsaLive.Core.Models.Generics;
    using Core.ResultConstants;
    using DAL.Entities;
    using Dtos.Auth;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Options;

    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IResetPasswordTokenProvider _resetPasswordTokenProvider;
        private readonly AccessTokenOptions _tokenOptions;

        public AuthService(
            UserManager<User> userManager,
            IEmailService emailService,
            IResetPasswordTokenProvider resetPasswordTokenProvider,
            IOptions<AccessTokenOptions> tokenOption)
        {
            ValidateTokenOptions(tokenOption.Value);
            _userManager = userManager;
            _emailService = emailService;
            _resetPasswordTokenProvider = resetPasswordTokenProvider;
            _tokenOptions = tokenOption.Value;
        }

        public async Task<IResult> RegisterUserAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken)
        {
            try
            {
                if (await _userManager.Users.AnyAsync(u => u.Email == registerUserDto.Email, cancellationToken))
                    return Result.CreateFailed(AuthServiceResultConstants.UserAlreadyExists);

                var user = new User
                {
                    Email = registerUserDto.Email,
                    UserName = Guid.NewGuid().ToString()
                };

                var createUser = await _userManager.CreateAsync(user, registerUserDto.Password);
                if (!createUser.Succeeded)
                    Result.CreateFailed(createUser.Errors.Select(e => e.Description));

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<IResult<TokenDto>> GetTokenAsync(LoginUserDto loginUserDto, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager
                    .Users
                    .FirstOrDefaultAsync(u => u.Email == loginUserDto.Email, cancellationToken);

                if (user == null)
                    return Result<TokenDto>.CreateFailed(AuthServiceResultConstants.UserNotFound);

                if (!await _userManager.CheckPasswordAsync(user, loginUserDto.Password))
                    return Result<TokenDto>.CreateFailed(AuthServiceResultConstants.InvalidUserNameOrPassword);

                return Result<TokenDto>.CreateSuccess(GenerateTokenForUser(user));
            }
            catch (Exception e)
            {
                return Result<TokenDto>.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<IResult> SendPasswordResetTokenAsync(string email, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager
                    .Users
                    .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

                if (user == null)
                    return Result.CreateFailed(AuthServiceResultConstants.UserNotFound);

                var resetToken = _resetPasswordTokenProvider.GenerateResetToken(user.Id);
                var sendResult = await _emailService.SendAsync(user.Email, resetToken.Code);

                return !sendResult.Success
                    ? Result.CreateFailed(CommonResultConstants.Unexpected)
                    : Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<IResult> VerifyResetPasswordTokenAsync(
            string token,
            string email,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager
                    .Users
                    .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

                if (user == null)
                    return Result.CreateFailed(AuthServiceResultConstants.UserNotFound);

                var verifyResult = _resetPasswordTokenProvider.VerifyResetToken(token, user.Id);

                if (!verifyResult.Success)
                    return verifyResult;

                return Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<IResult> ResetPasswordAsync(
            ResetPasswordDto resetPasswordDto,
            CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager
                    .Users
                    .FirstOrDefaultAsync(u => u.Email == resetPasswordDto.Email, cancellationToken);

                if (user == null)
                    return Result.CreateFailed(AuthServiceResultConstants.UserNotFound);

                var verifyResult = _resetPasswordTokenProvider.VerifyResetToken(resetPasswordDto.Token, user.Id);

                if (!verifyResult.Success)
                    return verifyResult;

                await _userManager.RemovePasswordAsync(user);
                var resetResult = await _userManager.AddPasswordAsync(user, resetPasswordDto.NewPassword);

                return !resetResult.Succeeded
                    ? Result.CreateFailed(resetResult.Errors.Select(e => e.Description))
                    : Result.CreateSuccess();
            }
            catch (Exception e)
            {
                return Result.CreateFailed(CommonResultConstants.Unexpected, e);
            }
        }

        public async Task<IResult<TokenDto>> RefreshTokenAsync(string refreshToken)
        {
            var principalResult = GetPrincipalFromExpiredToken(refreshToken);

            if (!principalResult.Success)
                return Result<TokenDto>.CreateFailed(principalResult.Messages, principalResult.Exception);

            if (principalResult.Data.Item2.Subject == null)
                return Result<TokenDto>.CreateFailed(AuthServiceResultConstants.InvalidRefreshToken);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(principalResult.Data.Item2.Subject));

            if (user == null)
                return Result<TokenDto>.CreateFailed(AuthServiceResultConstants.UserNotFound);

            return Result<TokenDto>.CreateSuccess(GenerateTokenForUser(user));
        }

        private static ClaimsIdentity GetRefreshClaimsIdentity(User user)
            => new(new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString())
            });

        private static void ValidateTokenOptions(AccessTokenOptions accessTokenOptions)
        {
            if(string.IsNullOrWhiteSpace(accessTokenOptions.Audience))
                throw new ArgumentException($"Please specify {nameof(accessTokenOptions.Audience)}", nameof(accessTokenOptions.Audience));
            if(string.IsNullOrWhiteSpace(accessTokenOptions.Issuer))
                throw new ArgumentException($"Please specify {nameof(accessTokenOptions.Issuer)}", nameof(accessTokenOptions.Issuer));
            if(string.IsNullOrWhiteSpace(accessTokenOptions.Key))
                throw new ArgumentException($"Please specify {nameof(accessTokenOptions.Key)}", nameof(accessTokenOptions.Key));
        }

        private ClaimsIdentity GetClaimsIdentity(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email)
            };

            if (user.PhoneNumber != null)
                claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));

            return new ClaimsIdentity(claims, "Token");
        }

        private TokenDto GenerateTokenForUser(User user)
        {
            var accessToken = GenerateToken(user);
            var refreshToken = GenerateRefreshToken(user);
            var jwtHandler = new JwtSecurityTokenHandler();

            return new TokenDto(
                jwtHandler.WriteToken(accessToken),
                accessToken.ValidTo,
                jwtHandler.WriteToken(refreshToken),
                refreshToken.ValidTo);
        }

        private IResult<(ClaimsPrincipal, JwtSecurityToken)> GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(
                        token,
                        new TokenValidationParameters
                        {
                            ValidateActor = _tokenOptions.ValidateActor,
                            ValidateAudience = _tokenOptions.ValidateAudience,
                            ValidateLifetime = _tokenOptions.ValidateLifetime,
                            ValidateIssuerSigningKey = _tokenOptions.ValidateIssuerSigningKey,
                            ValidIssuer = _tokenOptions.Issuer,
                            ValidAudience = _tokenOptions.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.Key!))
                        },
                        out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                    !jwtSecurityToken.Header.Alg.Equals(
                        SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                    return Result<(ClaimsPrincipal, JwtSecurityToken)>.CreateFailed(AuthServiceResultConstants
                        .InvalidRefreshToken);

                return Result<(ClaimsPrincipal, JwtSecurityToken)>.CreateSuccess((principal, jwtSecurityToken));
            }
            catch (Exception e)
            {
                return Result<(ClaimsPrincipal, JwtSecurityToken)>.CreateFailed(AuthServiceResultConstants.InvalidRefreshToken, e);
            }
        }

        private JwtSecurityToken GenerateToken(User user)
        {
            var claims = GetClaimsIdentity(user);

            return new JwtSecurityToken(
                _tokenOptions.Issuer,
                _tokenOptions.Audience,
                notBefore: DateTime.UtcNow,
                claims: claims.Claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_tokenOptions.AccessTokenLifetime)),
                signingCredentials: GetSignInCredentials());
        }

        private JwtSecurityToken GenerateRefreshToken(User user)
        {
            var claims = GetRefreshClaimsIdentity(user);

            return new JwtSecurityToken(
                _tokenOptions.Issuer,
                _tokenOptions.Audience,
                notBefore: DateTime.UtcNow,
                claims: claims.Claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_tokenOptions.RefreshTokenLifetime)),
                signingCredentials: GetSignInCredentials());
        }

        private SigningCredentials GetSignInCredentials()
            => new(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenOptions.Key!)),
                SecurityAlgorithms.HmacSha256);
    }
}