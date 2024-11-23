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

public class VisitorServiceTests
{
    private readonly Mock<IVisitorRepository> _mockVisitorRepository;
    private readonly VisitorService _visitorService;
    private readonly DateTime _fixedDate;

    public VisitorServiceTests()
    {
        _mockVisitorRepository = new Mock<IVisitorRepository>();
        _visitorService = new VisitorService(_mockVisitorRepository.Object);
        _fixedDate = new DateTime(2024, 1, 1);
    }

    [Fact]
    public async Task GetAllVisitorsAsync_ShouldReturnAllVisitors()
    {
        // Arrange
        var expectedVisitors = new List<Visitor>
        {
            new Visitor
            {
                Id = 1,
                Name = "John Doe",
                Email = "john@example.com",
                PhoneNumber = "1234567890",
                Identification = "12345678",
                CreatedAt = _fixedDate,
                UpdatedAt = _fixedDate
            },
            new Visitor
            {
                Id = 2,
                Name = "Jane Doe",
                Email = "jane@example.com",
                PhoneNumber = "0987654321",
                Identification = "87654321",
                CreatedAt = _fixedDate,
                UpdatedAt = _fixedDate
            }
        };

        _mockVisitorRepository
            .Setup(x => x.GetAllVisitorsAsync())
            .ReturnsAsync(expectedVisitors);

        // Act
        var result = await _visitorService.GetAllVisitorsAsync();

        // Assert
        Assert.Equal(expectedVisitors, result);
        _mockVisitorRepository.Verify(x => x.GetAllVisitorsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetVisitorByIdAsync_ShouldReturnVisitor()
    {
        // Arrange
        var expectedVisitor = new Visitor
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            Identification = "12345678",
            CreatedAt = _fixedDate,
            UpdatedAt = _fixedDate
        };

        _mockVisitorRepository
            .Setup(x => x.GetVisitorByIdAsync(1))
            .ReturnsAsync(expectedVisitor);

        // Act
        var result = await _visitorService.GetVisitorByIdAsync(1);

        // Assert
        Assert.Equal(expectedVisitor, result);
        _mockVisitorRepository.Verify(x => x.GetVisitorByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task CreateVisitorAsync_ShouldReturnNewId()
    {
        // Arrange
        var newVisitor = new Visitor
        {
            Name = "John Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            Identification = "12345678",
            CreatedAt = _fixedDate,
            UpdatedAt = _fixedDate
        };
        var expectedId = 1;

        _mockVisitorRepository
            .Setup(x => x.CreateVisitorAsync(It.Is<Visitor>(v =>
                v.Name == newVisitor.Name &&
                v.Email == newVisitor.Email &&
                v.Identification == newVisitor.Identification)))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _visitorService.CreateVisitorAsync(newVisitor);

        // Assert
        Assert.Equal(expectedId, result);
        _mockVisitorRepository.Verify(x => x.CreateVisitorAsync(newVisitor), Times.Once);
    }

    [Fact]
    public async Task UpdateVisitorAsync_ShouldReturnTrue()
    {
        // Arrange
        var visitor = new Visitor
        {
            Id = 1,
            Name = "John Doe Updated",
            Email = "john.updated@example.com",
            PhoneNumber = "1234567890",
            Identification = "12345678",
            CreatedAt = _fixedDate,
            UpdatedAt = _fixedDate
        };

        _mockVisitorRepository
            .Setup(x => x.UpdateVisitorAsync(It.Is<Visitor>(v =>
                v.Id == visitor.Id &&
                v.Name == visitor.Name)))
            .ReturnsAsync(true);

        // Act
        var result = await _visitorService.UpdateVisitorAsync(visitor);

        // Assert
        Assert.True(result);
        _mockVisitorRepository.Verify(x => x.UpdateVisitorAsync(visitor), Times.Once);
    }

    [Fact]
    public async Task UpdateVisitorAsync_NonExisting_ShouldReturnFalse()
    {
        // Arrange
        var visitor = new Visitor
        {
            Id = 999,
            Name = "Non-existing Visitor",
            Identification = "99999999",
            CreatedAt = _fixedDate,
            UpdatedAt = _fixedDate
        };

        _mockVisitorRepository
            .Setup(x => x.UpdateVisitorAsync(visitor))
            .ReturnsAsync(false);

        // Act
        var result = await _visitorService.UpdateVisitorAsync(visitor);

        // Assert
        Assert.False(result);
        _mockVisitorRepository.Verify(x => x.UpdateVisitorAsync(visitor), Times.Once);
    }

    [Fact]
    public async Task DeleteVisitorAsync_ShouldReturnTrue()
    {
        // Arrange
        _mockVisitorRepository
            .Setup(x => x.DeleteVisitorAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _visitorService.DeleteVisitorAsync(1);

        // Assert
        Assert.True(result);
        _mockVisitorRepository.Verify(x => x.DeleteVisitorAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteVisitorAsync_NonExisting_ShouldReturnFalse()
    {
        // Arrange
        _mockVisitorRepository
            .Setup(x => x.DeleteVisitorAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _visitorService.DeleteVisitorAsync(999);

        // Assert
        Assert.False(result);
        _mockVisitorRepository.Verify(x => x.DeleteVisitorAsync(999), Times.Once);
    }
}