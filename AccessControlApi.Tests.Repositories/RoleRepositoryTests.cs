using AccessControlApi.Domain.Entities;
using AccessControlApi.Infrastructure.Repositories;
using AccessControlApi.Infrastructure.Wrappers;
using Moq;
using System.Data;

namespace AccessControlApi.Tests.Repositories;

public class RoleRepositoryTests
{
    private readonly Mock<IDbConnection> _mockDbConnection;
    private readonly Mock<IDapperWrapper> _mockDapper;
    private readonly RoleRepository _roleRepository;
    private readonly DateTime _fixedDate;

    public RoleRepositoryTests()
    {
        _mockDbConnection = new Mock<IDbConnection>();
        _mockDapper = new Mock<IDapperWrapper>();
        _roleRepository = new RoleRepository(_mockDbConnection.Object, _mockDapper.Object);
        _fixedDate = new DateTime(2024, 1, 1);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRoles()
    {
        // Arrange
        var expectedRoles = new List<Role>
        {
            new Role { Id = 1, Name = "Admin", Description = "Administrator" },
            new Role { Id = 2, Name = "User", Description = "Regular User" }
        };

        _mockDapper
            .Setup(x => x.QueryAsync<IEnumerable<Role>>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                null))
            .ReturnsAsync(expectedRoles);

        // Act
        var result = await _roleRepository.GetAllAsync();

        // Assert
        Assert.Equal(expectedRoles, result);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingRole_ShouldReturnRole()
    {
        // Arrange
        var expectedRole = new Role
        {
            Id = 1,
            Name = "Admin",
            Description = "Administrator"
        };

        _mockDapper
            .Setup(x => x.QuerySingleOrDefaultAsync<Role>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<object>(p => p.GetType().GetProperty("Id").GetValue(p).Equals(1))))
            .ReturnsAsync(expectedRole);

        // Act
        var result = await _roleRepository.GetByIdAsync(1);

        // Assert
        Assert.Equal(expectedRole, result);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingRole_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _mockDapper
            .Setup(x => x.QuerySingleOrDefaultAsync<Role>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((Role)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _roleRepository.GetByIdAsync(999));
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNewId()
    {
        // Arrange
        var newRole = new Role
        {
            Name = "New Role",
            Description = "New Role Description"
        };
        var expectedId = 1;

        _mockDapper
            .Setup(x => x.QuerySingleAsync<int>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<Role>(r => r.Name == newRole.Name && r.Description == newRole.Description)))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _roleRepository.CreateAsync(newRole);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldExecuteSuccessfully()
    {
        // Arrange
        var role = new Role
        {
            Id = 1,
            Name = "Updated Role",
            Description = "Updated Description"
        };

        _mockDapper
            .Setup(x => x.ExecuteAsync(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<Role>(r => r.Id == role.Id)))
            .ReturnsAsync(1);

        // Act & Assert
        await _roleRepository.UpdateAsync(role);

        _mockDapper.Verify(x => x.ExecuteAsync(
            _mockDbConnection.Object,
            It.IsAny<string>(),
            It.Is<Role>(r => r.Id == role.Id)),
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
        await _roleRepository.DeleteAsync(1);

        // Assert
        _mockDapper.Verify(x => x.ExecuteAsync(
            _mockDbConnection.Object,
            It.IsAny<string>(),
            It.Is<object>(p => p.GetType().GetProperty("Id").GetValue(p).Equals(1))),
            Times.Once);
    }

    [Fact]
    public async Task GetRoleByIdAsync_ExistingRole_ShouldReturnRole()
    {
        // Arrange
        var expectedRole = new Role
        {
            Id = 1,
            Name = "Admin",
            Description = "Administrator"
        };

        _mockDapper
            .Setup(x => x.QuerySingleOrDefaultAsync<Role>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<object>(p => p.GetType().GetProperty("Id").GetValue(p).Equals(1))))
            .ReturnsAsync(expectedRole);

        // Act
        var result = await _roleRepository.GetRoleByIdAsync(1);

        // Assert
        Assert.Equal(expectedRole, result);
    }

    [Fact]
    public async Task GetRoleByIdAsync_NonExistingRole_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        _mockDapper
            .Setup(x => x.QuerySingleOrDefaultAsync<Role>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((Role)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _roleRepository.GetRoleByIdAsync(999));
    }
}
