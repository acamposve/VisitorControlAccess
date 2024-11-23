using AccessControlApi.Application.Services;
using AccessControlApi.Domain.Entities;
using AccessControlApi.Domain.InfrastructureInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AccessControlApi.Tests.Application;

public class AuthorizationServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthorizationService _authService;
    private readonly string _testSecretKey;

    public AuthorizationServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockConfiguration = new Mock<IConfiguration>();
        _testSecretKey = "ThisIsAVeryLongSecretKeyForSecurityPurposes123!@#";

        // Setup Configuration
        _mockConfiguration.Setup(x => x["Jwt:Secret"]).Returns(_testSecretKey);
        _mockConfiguration.Setup(x => x["Jwt:ExpirationDays"]).Returns("7");

        _authService = new AuthorizationService(
            _mockUserRepository.Object,
            _mockRoleRepository.Object,
            _mockConfiguration.Object);
    }

    [Fact]
    public async Task HasAccessAsync_WithValidUserAndMatchingRole_ShouldReturnTrue()
    {
        // Arrange
        var userId = 1;
        var roleId = 1;
        var requiredRole = "Admin";

        var user = new User { Id = userId, RoleId = roleId };
        var role = new Role { Id = roleId, Name = requiredRole };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockRoleRepository.Setup(x => x.GetRoleByIdAsync(roleId)).ReturnsAsync(role);

        // Act
        var result = await _authService.HasAccessAsync(userId, requiredRole);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasAccessAsync_WithNonMatchingRole_ShouldReturnFalse()
    {
        // Arrange
        var userId = 1;
        var roleId = 1;
        var requiredRole = "Admin";

        var user = new User { Id = userId, RoleId = roleId };
        var role = new Role { Id = roleId, Name = "User" };

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockRoleRepository.Setup(x => x.GetRoleByIdAsync(roleId)).ReturnsAsync(role);

        // Act
        var result = await _authService.HasAccessAsync(userId, requiredRole);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task HasAccessAsync_WithNonExistentUser_ShouldReturnFalse()
    {
        // Arrange
        _mockUserRepository.Setup(x => x.GetByIdAsync(999)).ReturnsAsync((User)null);

        // Act
        var result = await _authService.HasAccessAsync(999, "Admin");

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(0, "Admin")]
    [InlineData(-1, "Admin")]
    public async Task HasAccessAsync_WithInvalidUserId_ShouldThrowArgumentException(int userId, string role)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _authService.HasAccessAsync(userId, role));
    }

    [Theory]
    [InlineData(1, null)]
    [InlineData(1, "")]
    [InlineData(1, " ")]
    public async Task HasAccessAsync_WithInvalidRole_ShouldThrowArgumentException(int userId, string role)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _authService.HasAccessAsync(userId, role));
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccessfulResult()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Username = "test@example.com",
            Password = "password123"
        };

        var user = new User
        {
            Id = 1,
            Email = loginRequest.Username
        };

        _mockUserRepository
            .Setup(x => x.GetUserByCredentialsAsync(loginRequest.Username, loginRequest.Password))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Token);
        Assert.NotEmpty(result.Token);
        Assert.Equal(user.Id, result.UserId);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ShouldReturnFailureResult()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Username = "invalid@example.com",
            Password = "wrongpassword"
        };

        _mockUserRepository
            .Setup(x => x.GetUserByCredentialsAsync(loginRequest.Username, loginRequest.Password))
            .ReturnsAsync((User)null);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Null(result.Token);
        Assert.Equal("Invalid credentials", result.ErrorMessage);
    }

    [Theory]
    [InlineData(null, "password")]
    [InlineData("", "password")]
    [InlineData(" ", "password")]
    [InlineData("user", null)]
    [InlineData("user", "")]
    [InlineData("user", " ")]
    public async Task LoginAsync_WithInvalidRequest_ShouldThrowArgumentException(string username, string password)
    {
        // Arrange
        var request = new LoginRequest { Username = username, Password = password };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _authService.LoginAsync(request));
    }

    [Fact]
    public async Task RefreshToken_WithValidToken_ShouldReturnNewToken()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@example.com" };
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_testSecretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var validToken = tokenHandler.WriteToken(token);

        _mockUserRepository.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);

        var refreshRequest = new RefreshTokenRequest { Token = validToken };

        // Act
        var result = await _authService.RefreshToken(refreshRequest);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Token);
        Assert.NotEmpty(result.Token);
        Assert.Equal(user.Id, result.UserId);
        Assert.Null(result.ErrorMessage);
        Assert.NotEqual(validToken, result.Token);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ShouldReturnFailureResult()
    {
        // Arrange
        var refreshRequest = new RefreshTokenRequest { Token = "invalid_token" };

        // Act
        var result = await _authService.RefreshToken(refreshRequest);

        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Null(result.Token);
        Assert.Equal("Invalid token", result.ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task RefreshToken_WithInvalidRequest_ShouldThrowArgumentException(string token)
    {
        // Arrange
        var request = new RefreshTokenRequest { Token = token };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _authService.RefreshToken(request));
    }
}