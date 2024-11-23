using AccessControlApi.Domain.Entities;
using AccessControlApi.Infrastructure.Repositories;
using AccessControlApi.Infrastructure.Wrappers;
using Moq;
using System.Data;

namespace AccessControlApi.Tests.Repositories;

public class UserRepositoryTests
{
    private readonly Mock<IDbConnection> _mockDbConnection;
    private readonly Mock<IDapperWrapper> _mockDapper;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        _mockDbConnection = new Mock<IDbConnection>();
        _mockDapper = new Mock<IDapperWrapper>();
        _userRepository = new UserRepository(_mockDbConnection.Object, _mockDapper.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var expectedUsers = new List<User>
        {
            new User
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@example.com",
                RoleId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                Name = "Jane Doe",
                Email = "jane@example.com",
                RoleId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        _mockDapper
            .Setup(x => x.QueryAsync<IEnumerable<User>>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                null))
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _userRepository.GetAllAsync();

        // Assert
        Assert.Equal(expectedUsers, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser()
    {
        // Arrange
        var expectedUser = new User
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockDapper
            .Setup(x => x.QuerySingleOrDefaultAsync<User>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<object>(p => p.GetType().GetProperty("Id").GetValue(p).Equals(1))))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userRepository.GetByIdAsync(1);

        // Assert
        Assert.Equal(expectedUser, result);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNewId()
    {
        // Arrange
        var newUser = new User
        {
            Name = "John Doe",
            Email = "john@example.com",
            PasswordHash = "hashedPassword123",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var expectedId = 1;

        _mockDapper
            .Setup(x => x.QuerySingleAsync<int>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<User>(u => u.Name == newUser.Name && u.Email == newUser.Email)))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _userRepository.CreateAsync(newUser);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task GetUserByCredentialsAsync_ShouldReturnUser()
    {
        // Arrange
        var email = "john@example.com";
        var passwordHash = "hashedPassword123";
        var expectedUser = new User
        {
            Id = 1,
            Name = "John Doe",
            Email = email,
            PasswordHash = passwordHash,
            RoleId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockDapper
            .Setup(x => x.QuerySingleOrDefaultAsync<User>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<object>(p =>
                    p.GetType().GetProperty("Email").GetValue(p).Equals(email) &&
                    p.GetType().GetProperty("PasswordHash").GetValue(p).Equals(passwordHash))))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userRepository.GetUserByCredentialsAsync(email, passwordHash);

        // Assert
        Assert.Equal(expectedUser, result);
        Assert.Equal(email, result.Email);
        Assert.Equal(passwordHash, result.PasswordHash);
    }

    [Fact]
    public async Task UpdateAsync_ShouldExecuteSuccessfully()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            PasswordHash = "hashedPassword123",
            RoleId = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockDapper
            .Setup(x => x.ExecuteAsync(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<User>(u => u.Id == user.Id)))
            .ReturnsAsync(1);

        // Act
        await _userRepository.UpdateAsync(user);

        // Assert
        _mockDapper.Verify(x => x.ExecuteAsync(
            _mockDbConnection.Object,
            It.IsAny<string>(),
            It.Is<User>(u => u.Id == user.Id)),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldExecuteSuccessfully()
    {
        // Arrange
        _mockDapper
            .Setup(x => x.ExecuteAsync(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<object>(p => p.GetType().GetProperty("Id").GetValue(p).Equals(1))))
            .ReturnsAsync(1);

        // Act
        await _userRepository.DeleteAsync(1);

        // Assert
        _mockDapper.Verify(x => x.ExecuteAsync(
            _mockDbConnection.Object,
            It.IsAny<string>(),
            It.Is<object>(p => p.GetType().GetProperty("Id").GetValue(p).Equals(1))),
            Times.Once);
    }
}