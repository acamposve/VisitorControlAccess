using AccessControlApi.Application.Services;
using AccessControlApi.Domain.Entities;
using AccessControlApi.Domain.Exceptions;
using AccessControlApi.Domain.InfrastructureInterfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControlApi.Tests.Application;

public class AccessPointServiceTests
{
    private readonly Mock<IAccessPointRepository> _mockRepository;
    private readonly AccessPointService _service;

    public AccessPointServiceTests()
    {
        _mockRepository = new Mock<IAccessPointRepository>();
        _service = new AccessPointService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllAccessPointsAsync_ShouldReturnAllAccessPoints()
    {
        // Arrange
        var expectedAccessPoints = new List<AccessPoint>
        {
            new AccessPoint { Id = 1, Name = "Main Gate", Description = "Main entrance", CreatedBy = 1 },
            new AccessPoint { Id = 2, Name = "Side Gate", Description = "Side entrance", CreatedBy = 1 }
        };

        _mockRepository
            .Setup(x => x.GetAllAccessPointsAsync())
            .ReturnsAsync(expectedAccessPoints);

        // Act
        var result = await _service.GetAllAccessPointsAsync();

        // Assert
        Assert.Equal(expectedAccessPoints, result);
        _mockRepository.Verify(x => x.GetAllAccessPointsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAccessPointByIdAsync_WithValidId_ShouldReturnAccessPoint()
    {
        // Arrange
        var expectedAccessPoint = new AccessPoint
        {
            Id = 1,
            Name = "Main Gate",
            Description = "Main entrance",
            CreatedBy = 1
        };

        _mockRepository
            .Setup(x => x.GetAccessPointByIdAsync(1))
            .ReturnsAsync(expectedAccessPoint);

        // Act
        var result = await _service.GetAccessPointByIdAsync(1);

        // Assert
        Assert.Equal(expectedAccessPoint, result);
        _mockRepository.Verify(x => x.GetAccessPointByIdAsync(1), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetAccessPointByIdAsync_WithInvalidId_ShouldThrowArgumentException(int id)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.GetAccessPointByIdAsync(id));
    }

    [Fact]
    public async Task GetAccessPointByIdAsync_WithNonExistingId_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockRepository
            .Setup(x => x.GetAccessPointByIdAsync(1))
            .ReturnsAsync((AccessPoint)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.GetAccessPointByIdAsync(1));
    }

    [Fact]
    public async Task CreateAccessPointAsync_WithValidAccessPoint_ShouldReturnId()
    {
        // Arrange
        var accessPoint = new AccessPoint
        {
            Name = "New Gate",
            Description = "New entrance",
            CreatedBy = 1
        };
        var expectedId = 1;

        _mockRepository
            .Setup(x => x.CreateAccessPointAsync(accessPoint))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _service.CreateAccessPointAsync(accessPoint);

        // Assert
        Assert.Equal(expectedId, result);
        _mockRepository.Verify(x => x.CreateAccessPointAsync(accessPoint), Times.Once);
    }

    [Fact]
    public async Task CreateAccessPointAsync_WithNullAccessPoint_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.CreateAccessPointAsync(null));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task CreateAccessPointAsync_WithInvalidName_ShouldThrowArgumentException(string name)
    {
        // Arrange
        var accessPoint = new AccessPoint { Name = name, CreatedBy = 1 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAccessPointAsync(accessPoint));
    }

    [Fact]
    public async Task UpdateAccessPointAsync_WithValidAccessPoint_ShouldReturnTrue()
    {
        // Arrange
        var accessPoint = new AccessPoint
        {
            Id = 1,
            Name = "Updated Gate",
            Description = "Updated entrance",
            CreatedBy = 1
        };

        _mockRepository
            .Setup(x => x.UpdateAccessPointAsync(accessPoint))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateAccessPointAsync(accessPoint);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(x => x.UpdateAccessPointAsync(accessPoint), Times.Once);
    }

    [Fact]
    public async Task UpdateAccessPointAsync_WithNonExistingId_ShouldThrowNotFoundException()
    {
        // Arrange
        var accessPoint = new AccessPoint
        {
            Id = 999,
            Name = "Non-existing Gate",
            CreatedBy = 1
        };

        _mockRepository
            .Setup(x => x.UpdateAccessPointAsync(accessPoint))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.UpdateAccessPointAsync(accessPoint));
    }

    [Fact]
    public async Task DeleteAccessPointAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        _mockRepository
            .Setup(x => x.DeleteAccessPointAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAccessPointAsync(1);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(x => x.DeleteAccessPointAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAccessPointAsync_WithNonExistingId_ShouldThrowNotFoundException()
    {
        // Arrange
        _mockRepository
            .Setup(x => x.DeleteAccessPointAsync(999))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.DeleteAccessPointAsync(999));
    }
}