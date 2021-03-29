namespace DummyWebApp.Tests.ServicesTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using BLL.Dtos.Auth;
    using BLL.Options;
    using BLL.Services;
    using BLL.Services.Abstraction;
    using DAL.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MockQueryable.Moq;
    using Moq;
    using Xunit;

    public class AuthServiceTests
    {
        private readonly IAuthService _authService;
        private readonly Mock<UserStore> _userStoreMock;

        public AuthServiceTests()
        {
            _userStoreMock = new Mock<UserStore>();

            var passHasherMock = new Mock<IPasswordHasher<User>>();
            var optionsMock = new Mock<IOptions<AccessTokenOptions>>();

            optionsMock
                .SetupGet(options => options.Value)
                .Returns(new AccessTokenOptions
                {
                    Audience = Guid.NewGuid().ToString(),
                    Issuer = Guid.NewGuid().ToString(),
                    Key = Guid.NewGuid().ToString()
                });
            passHasherMock
                .Setup(hasher => hasher.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(PasswordVerificationResult.Success);
            _userStoreMock
                .Setup(store => store.GetPasswordHashAsync(It.IsAny<User>(), CancellationToken.None))
                .Returns(() => Task.FromResult(string.Empty));
            
            var userManager = new UserManager<User>(
                _userStoreMock.Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                passHasherMock.Object,
                Enumerable.Empty<IUserValidator<User>>(),
                Enumerable.Empty<IPasswordValidator<User>>(),
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);
            
            _authService = new AuthService(
                userManager,
                new Mock<IEmailService>().Object,
                new Mock<IResetPasswordTokenProvider>().Object,
                optionsMock.Object);
        }

        [InlineData("test@gmail.com", "testPass")]
        [InlineData("test2@gmail.com", "testPass2")]
        [InlineData("test3@gmail.com", "testPass3")]
        [Theory]
        public async Task ShouldGenerateTokenForUser(string email, string password)
        {
            var users = new List<User>
            {
                new()
                {
                    Email = email,
                    PasswordHash = password,
                    Id = new Random().Next(1, 100)
                }
            };
            var usersMock = users.AsQueryable().BuildMock();
            
            _userStoreMock.SetupGet(store => store.Users).Returns(usersMock.Object);
            
            var tokenResult = await _authService.GetTokenAsync(new LoginUserDto(email, password));
            
            Assert.True(tokenResult.Success);
            Assert.NotNull(tokenResult.Data);
            Assert.NotNull(tokenResult.Data.Token);
            Assert.NotNull(tokenResult.Data.RefreshToken);
        }
        
        [InlineData("test@gmail.com", "testPass")]
        [InlineData("test2@gmail.com", "testPass2")]
        [InlineData("test3@gmail.com", "testPass3")]
        [Theory]
        public async Task ShouldRefreshToken(string email, string password)
        {
            var users = new List<User>
            {
                new()
                {
                    Email = email,
                    PasswordHash = password,
                    Id = new Random().Next(1, 100)
                }
            };
            var usersMock = users.AsQueryable().BuildMock();
            
            _userStoreMock.SetupGet(store => store.Users).Returns(usersMock.Object);
            
            var tokenResult = await _authService.GetTokenAsync(new LoginUserDto(email, password));
            var refreshResult = await _authService.RefreshTokenAsync(tokenResult.Data.RefreshToken);
            
            Assert.True(refreshResult.Success);
            Assert.NotNull(refreshResult.Data);
            Assert.NotNull(refreshResult.Data.Token);
            Assert.NotNull(refreshResult.Data.RefreshToken);
        }
        
        [InlineData("test@gmail.com", "testPass")]
        [InlineData("test2@gmail.com", "testPass2")]
        [InlineData("test3@gmail.com", "testPass3")]
        [Theory]
        public async Task ShouldNotRefreshTokenWithInvalidRefreshToken(string email, string password)
        {
            var users = new List<User>
            {
                new()
                {
                    Email = email,
                    PasswordHash = password,
                    Id = new Random().Next(1, 100)
                }
            };
            var usersMock = users.AsQueryable().BuildMock();
            var randomRefreshToken = Guid.NewGuid().ToString();
            
            _userStoreMock.SetupGet(store => store.Users).Returns(usersMock.Object);
            
            var refreshResult = await _authService.RefreshTokenAsync(randomRefreshToken);
            
            Assert.False(refreshResult.Success);
        }

        [InlineData("test@gmail.com", "testPass")]
        [InlineData("test2@gmail.com", "testPass2")]
        [InlineData("test3@gmail.com", "testPass3")]
        [Theory]
        public async Task ShouldRegisterNewUser(string email, string password)
        {
            var users = new List<User>
            {
                new()
                {
                    Email = Guid.NewGuid().ToString(),
                    PasswordHash = Guid.NewGuid().ToString(),
                    Id = new Random().Next(1, 100)
                }
            };
            var usersMock = users.AsQueryable().BuildMock();
            
            _userStoreMock.SetupGet(store => store.Users).Returns(usersMock.Object);
            _userStoreMock
                .Setup(store => store.CreateAsync(It.IsAny<User>(), CancellationToken.None))
                .Returns(() => Task.FromResult(new IdentityResult()));

            var registerResult = await _authService.RegisterUserAsync(new RegisterUserDto(email, password));
            
            Assert.True(registerResult.Success);
            Assert.Empty(registerResult.Messages);
            Assert.Null(registerResult.Exception);
        }
        
        
        public abstract class UserStore : IQueryableUserStore<User>, IUserPasswordStore<User>
        {
            public abstract void Dispose();
            public abstract Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken);
            public abstract Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken);
            public abstract Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken);
            public abstract Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken);
            public abstract Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken);
            public abstract Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken);
            public abstract Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken);
            public abstract Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken);
            public abstract Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken);
            public abstract Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken);
            public abstract IQueryable<User> Users { get; }
            public abstract Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken);
            public abstract Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken);
            public abstract Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken);
        }
    }
}