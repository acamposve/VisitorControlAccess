using AccessControlApi.Application.Services;
using AccessControlApi.Domain.Entities;
using AccessControlApi.Domain.InfrastructureInterfaces;
using Moq;

namespace AccessControlApi.Tests.Application;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;
    private readonly DateTime _fixedDate;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockUserRepository.Object);
        _fixedDate = new DateTime(2024, 1, 1);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var expectedUsers = new List<User>
        {
            new User
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@example.com",
                PasswordHash = "hash1",
                RoleId = 1,
                CreatedAt = _fixedDate,
                UpdatedAt = _fixedDate
            },
            new User
            {
                Id = 2,
                Name = "Jane Doe",
                Email = "jane@example.com",
                PasswordHash = "hash2",
                RoleId = 2,
                CreatedAt = _fixedDate,
                UpdatedAt = _fixedDate
            }
        };

        _mockUserRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(expectedUsers);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.Equal(expectedUsers, result);
        _mockUserRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser()
    {
        // Arrange
        var expectedUser = new User
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            PasswordHash = "hash1",
            RoleId = 1,
            CreatedAt = _fixedDate,
            UpdatedAt = _fixedDate
        };

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        Assert.Equal(expectedUser, result);
        _mockUserRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ShouldReturnUser()
    {
        // Arrange
        var email = "john@example.com";
        var expectedUser = new User
        {
            Id = 1,
            Name = "John Doe",
            Email = email,
            PasswordHash = "hash1",
            RoleId = 1,
            CreatedAt = _fixedDate,
            UpdatedAt = _fixedDate
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

        // Assert
        Assert.Equal(expectedUser, result);
        _mockUserRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnNewId()
    {
        // Arrange
        var newUser = new User
        {
            Name = "John Doe",
            Email = "john@example.com",
            PasswordHash = "hash1",
            RoleId = 1,
            CreatedAt = _fixedDate,
            UpdatedAt = _fixedDate
        };
        var expectedId = 1;

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.Is<User>(u =>
                u.Name == newUser.Name &&
                u.Email == newUser.Email &&
                u.PasswordHash == newUser.PasswordHash)))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _userService.CreateUserAsync(newUser);

        // Assert
        Assert.Equal(expectedId, result);
        _mockUserRepository.Verify(x => x.CreateAsync(newUser), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldCallRepository()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Name = "John Doe Updated",
            Email = "john.updated@example.com",
            PasswordHash = "hash1",
            RoleId = 1,
            CreatedAt = _fixedDate,
            UpdatedAt = _fixedDate
        };

        _mockUserRepository
            .Setup(x => x.UpdateAsync(It.Is<User>(u =>
                u.Id == user.Id &&
                u.Name == user.Name &&
                u.Email == user.Email)))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.UpdateUserAsync(user);

        // Assert
        _mockUserRepository.Verify(x => x.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldCallRepository()
    {
        // Arrange
        var userId = 1;

        _mockUserRepository
            .Setup(x => x.DeleteAsync(userId))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.DeleteUserAsync(userId);

        // Assert
        _mockUserRepository.Verify(x => x.DeleteAsync(userId), Times.Once);
    }
}