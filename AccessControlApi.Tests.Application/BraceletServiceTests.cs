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

public class BraceletServiceTests
{
    private readonly Mock<IBraceletRepository> _mockBraceletRepository;
    private readonly BraceletService _braceletService;
    private readonly DateTime _fixedDate;

    public BraceletServiceTests()
    {
        _mockBraceletRepository = new Mock<IBraceletRepository>();
        _braceletService = new BraceletService(_mockBraceletRepository.Object);
        _fixedDate = new DateTime(2024, 1, 1);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBracelets()
    {
        // Arrange
        var expectedBracelets = new List<Bracelet>
        {
            new Bracelet
            {
                Id = 1,
                VisitorId = 1,
                AccessPoints = new List<AccessPoint>
                {
                    new AccessPoint { Id = 1, Name = "Entrada Principal" },
                    new AccessPoint { Id = 2, Name = "Sala de Reuniones" }
                },
                CreatedAt = _fixedDate,
                QrCode = "QR123456"
            },
            new Bracelet
            {
                Id = 2,
                VisitorId = 2,
                AccessPoints = new List<AccessPoint>
                {
                    new AccessPoint { Id = 3, Name = "Cafetería" }
                },
                CreatedAt = _fixedDate,
                QrCode = "QR789012"
            }
        };

        _mockBraceletRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(expectedBracelets);

        // Act
        var result = await _braceletService.GetAllAsync();

        // Assert
        Assert.Equal(expectedBracelets, result);
        _mockBraceletRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBracelet()
    {
        // Arrange
        var expectedBracelet = new Bracelet
        {
            Id = 1,
            VisitorId = 1,
            AccessPoints = new List<AccessPoint>
            {
                new AccessPoint { Id = 1, Name = "Entrada Principal" },
                new AccessPoint { Id = 2, Name = "Sala de Reuniones" }
            },
            CreatedAt = _fixedDate,
            QrCode = "QR123456"
        };

        _mockBraceletRepository
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(expectedBracelet);

        // Act
        var result = await _braceletService.GetByIdAsync(1);

        // Assert
        Assert.Equal(expectedBracelet.Id, result.Id);
        Assert.Equal(expectedBracelet.VisitorId, result.VisitorId);
        Assert.Equal(expectedBracelet.CreatedAt, result.CreatedAt);
        Assert.Equal(expectedBracelet.QrCode, result.QrCode);
        Assert.Equal(expectedBracelet.AccessPoints.Count, result.AccessPoints.Count);
        _mockBraceletRepository.Verify(x => x.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnNewId()
    {
        // Arrange
        var newBracelet = new Bracelet
        {
            VisitorId = 1,
            AccessPoints = new List<AccessPoint>
            {
                new AccessPoint { Id = 1, Name = "Entrada Principal" }
            },
            CreatedAt = _fixedDate,
            QrCode = "QR123456"
        };
        var expectedId = 1;

        _mockBraceletRepository
            .Setup(x => x.CreateAsync(It.Is<Bracelet>(b =>
                b.VisitorId == newBracelet.VisitorId &&
                b.QrCode == newBracelet.QrCode &&
                b.CreatedAt == newBracelet.CreatedAt)))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _braceletService.CreateAsync(newBracelet);

        // Assert
        Assert.Equal(expectedId, result);
        _mockBraceletRepository.Verify(x => x.CreateAsync(newBracelet), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldCallRepository()
    {
        // Arrange
        var bracelet = new Bracelet
        {
            Id = 1,
            VisitorId = 1,
            AccessPoints = new List<AccessPoint>
            {
                new AccessPoint { Id = 1, Name = "Entrada Principal" },
                new AccessPoint { Id = 2, Name = "Sala de Reuniones" },
                new AccessPoint { Id = 3, Name = "Cafetería" }
            },
            CreatedAt = _fixedDate,
            QrCode = "QR123456_Updated"
        };

        _mockBraceletRepository
            .Setup(x => x.UpdateAsync(It.Is<Bracelet>(b =>
                b.Id == bracelet.Id &&
                b.VisitorId == bracelet.VisitorId &&
                b.QrCode == bracelet.QrCode)))
            .Returns(Task.CompletedTask);

        // Act
        await _braceletService.UpdateAsync(bracelet);

        // Assert
        _mockBraceletRepository.Verify(x => x.UpdateAsync(bracelet), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepository()
    {
        // Arrange
        var braceletId = 1;

        _mockBraceletRepository
            .Setup(x => x.DeleteAsync(braceletId))
            .Returns(Task.CompletedTask);

        // Act
        await _braceletService.DeleteAsync(braceletId);

        // Assert
        _mockBraceletRepository.Verify(x => x.DeleteAsync(braceletId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExisting_ShouldReturnNull()
    {
        // Arrange
        _mockBraceletRepository
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Bracelet)null);

        // Act
        var result = await _braceletService.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockBraceletRepository.Verify(x => x.GetByIdAsync(999), Times.Once);
    }
}