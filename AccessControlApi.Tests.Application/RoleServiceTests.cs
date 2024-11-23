using AccessControlApi.Application.Services;
using AccessControlApi.Domain.Entities;
using AccessControlApi.Domain.InfrastructureInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControlApi.Tests.Application;

public class RoleServiceTests
{
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly RoleService _roleService;

    public RoleServiceTests()
    {
        _mockRoleRepository = new Mock<IRoleRepository>();
        _roleService = new RoleService(_mockRoleRepository.Object);
    }

    [Fact]
    public async Task GetAllRolesAsync_ShouldReturnAllRoles()
    {
        // Arrange
        var expectedRoles = new List<Role>
        {
            new Role
            {
                Id = 1,
                Name = "Administrador",
                Description = "Role de administrador del sistema",
                Users = new List<User>
                {
                    new User { Id = 1, Name = "Admin User" }
                }
            },
            new Role
            {
                Id = 2,
                Name = "Usuario",
                Description = "Role de usuario regular",
                Users = new List<User>()
            }
        };

        _mockRoleRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(expectedRoles);

        // Act
        var result = await _roleService.GetAllRolesAsync();

        // Assert
        Assert.Equal(expectedRoles, result);
        _mockRoleRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetRoleByIdAsync_ShouldReturnRole()
    {
        // Arrange
        var expectedRole = new Role
        {
            Id = 1,
            Name = "Administrador",
            Description = "Role de administrador del sistema",
            Users = new List<User>
            {
                new User { Id = 1, Name = "Admin User" }
            }
        };

        _mockRoleRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(expectedRole);

        // Act
        var result = await _roleService.GetRoleByIdAsync(1);

        // Assert
        Assert.Equal(expectedRole.Id, result.Id);
        Assert.Equal(expectedRole.Name, result.Name);
        Assert.Equal(expectedRole.Description, result.Description);
        Assert.Equal(expectedRole.Users.Count, result.Users.Count);
        _mockRoleRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task CreateRoleAsync_ShouldReturnNewId()
    {
        // Arrange
        var newRole = new Role
        {
            Name = "Manager",
            Description = "Role de gerente",
            Users = new List<User>()
        };
        var expectedId = 1;

        _mockRoleRepository
            .Setup(x => x.CreateAsync(It.Is<Role>(r =>
                r.Name == newRole.Name &&
                r.Description == newRole.Description)))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _roleService.CreateRoleAsync(newRole);

        // Assert
        Assert.Equal(expectedId, result);
        _mockRoleRepository.Verify(x => x.CreateAsync(newRole), Times.Once);
    }

    [Fact]
    public async Task UpdateRoleAsync_ShouldCallRepository()
    {
        // Arrange
        var role = new Role
        {
            Id = 1,
            Name = "Administrador Actualizado",
            Description = "Descripción actualizada del rol de administrador",
            Users = new List<User>
            {
                new User { Id = 1, Name = "Admin User" },
                new User { Id = 2, Name = "New Admin" }
            }
        };

        _mockRoleRepository
            .Setup(x => x.UpdateAsync(It.Is<Role>(r =>
                r.Id == role.Id &&
                r.Name == role.Name &&
                r.Description == role.Description)))
            .Returns(Task.CompletedTask);

        // Act
        await _roleService.UpdateRoleAsync(role);

        // Assert
        _mockRoleRepository.Verify(x => x.UpdateAsync(role), Times.Once);
    }

    [Fact]
    public async Task DeleteRoleAsync_ShouldCallRepository()
    {
        // Arrange
        var roleId = 1;

        _mockRoleRepository
            .Setup(x => x.DeleteAsync(roleId))
            .Returns(Task.CompletedTask);

        // Act
        await _roleService.DeleteRoleAsync(roleId);

        // Assert
        _mockRoleRepository.Verify(x => x.DeleteAsync(roleId), Times.Once);
    }

    [Fact]
    public async Task GetRoleByIdAsync_NonExisting_ShouldReturnNull()
    {
        // Arrange
        _mockRoleRepository
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Role)null);

        // Act
        var result = await _roleService.GetRoleByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockRoleRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
    }
}