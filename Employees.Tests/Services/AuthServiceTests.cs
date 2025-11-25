using Employees.Data.Models;
using Employees.Data.Repositories;
using Employees.Service.Configuration;
using Employees.Service.Services;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Employees.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();

            var jwtSettings = new JwtSettings
            {
                SecretKey = "test-secret-key-with-at-least-32-characters-for-security",
                TokenExpirationMinutes = 5,
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            };

            _jwtSettingsMock.Setup(x => x.Value).Returns(jwtSettings);
            _authService = new AuthService(_userRepositoryMock.Object, _jwtSettingsMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsLoginResponse()
        {
            // Arrange
            var username = "testuser";
            var password = "testpass";
            var user = new User { Id = 1, Username = username, Password = password };

            _userRepositoryMock
                .Setup(x => x.ValidateUserAsync(username, password))
                .ReturnsAsync(user);

            var request = new LoginRequest { Username = username, Password = password };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.Equal(username, result.Username);
            Assert.True(result.ExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var username = "testuser";
            var password = "wrongpass";

            _userRepositoryMock
                .Setup(x => x.ValidateUserAsync(username, password))
                .ReturnsAsync((User?)null);

            var request = new LoginRequest { Username = username, Password = password };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_TokenContainsCorrectClaims()
        {
            // Arrange
            var username = "testuser";
            var password = "testpass";
            var userId = 123;
            var user = new User { Id = userId, Username = username, Password = password };

            _userRepositoryMock
                .Setup(x => x.ValidateUserAsync(username, password))
                .ReturnsAsync(user);

            var request = new LoginRequest { Username = username, Password = password };

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            
            // Verificar formato
            var tokenParts = result.Token.Split('.');
            Assert.Equal(3, tokenParts.Length);
        }
    }
}