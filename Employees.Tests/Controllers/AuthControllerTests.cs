using Employees.API.Controllers;
using Employees.Data.Models;
using Employees.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Employees.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_authServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "testpass"
            };

            var loginResponse = new LoginResponse
            {
                Token = "test-jwt-token",
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                Username = request.Username
            };

            _authServiceMock
                .Setup(x => x.LoginAsync(request))
                .ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<LoginResponse>(okResult.Value);
            Assert.Equal(loginResponse.Token, returnedResponse.Token);
            Assert.Equal(request.Username, returnedResponse.Username);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "wrongpass"
            };

            _authServiceMock
                .Setup(x => x.LoginAsync(request))
                .ReturnsAsync((LoginResponse?)null);

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_EmptyUsername_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "",
                Password = "testpass"
            };

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_EmptyPassword_ReturnsBadRequest()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = ""
            };

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}