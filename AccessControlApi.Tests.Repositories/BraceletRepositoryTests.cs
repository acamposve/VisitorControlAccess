using AccessControlApi.Domain.Entities;
using AccessControlApi.Infrastructure.Repositories;
using AccessControlApi.Infrastructure.Wrappers;
using Moq;
using System.Data;

namespace AccessControlApi.Tests.Repositories;

public class BraceletRepositoryTests
{
    private readonly Mock<IDbConnection> _mockDbConnection;
    private readonly Mock<IDapperWrapper> _mockDapper;
    private readonly BraceletRepository _braceletRepository;
    private readonly DateTime _fixedDate;

    public BraceletRepositoryTests()
    {
        _mockDbConnection = new Mock<IDbConnection>();
        _mockDapper = new Mock<IDapperWrapper>();
        _braceletRepository = new BraceletRepository(_mockDbConnection.Object, _mockDapper.Object);
        _fixedDate = new DateTime(2024, 1, 1);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingBracelet_ShouldReturnBracelet()
    {
        // Arrange
        var expectedBracelet = new Bracelet
        {
            Id = 1,
            VisitorId = 1,
            AccessPoints = new List<AccessPoint>
            {
                new AccessPoint { Id = 1, Name = "Point A" },
                new AccessPoint { Id = 2, Name = "Point B" }
            },
            CreatedAt = _fixedDate,
            QrCode = "QR123"
        };

        _mockDapper
            .Setup(x => x.QuerySingleOrDefaultAsync<Bracelet>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<object>(p => p.GetType().GetProperty("Id").GetValue(p).Equals(1))))
            .ReturnsAsync(expectedBracelet);

        // Act
        var result = await _braceletRepository.GetByIdAsync(1);

        // Assert
        Assert.Equal(expectedBracelet.Id, result.Id);
        Assert.Equal(expectedBracelet.VisitorId, result.VisitorId);
        Assert.Equal(expectedBracelet.CreatedAt, result.CreatedAt);
        Assert.Equal(expectedBracelet.QrCode, result.QrCode);
        Assert.Equal(expectedBracelet.AccessPoints.Count, result.AccessPoints.Count);
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
                    new AccessPoint { Id = 1, Name = "Point A" },
                    new AccessPoint { Id = 2, Name = "Point B" }
                },
                CreatedAt = _fixedDate,
                QrCode = "QR123"
            },
            new Bracelet
            {
                Id = 2,
                VisitorId = 2,
                AccessPoints = new List<AccessPoint>
                {
                    new AccessPoint { Id = 3, Name = "Point C" }
                },
                CreatedAt = _fixedDate,
                QrCode = "QR456"
            }
        };

        _mockDapper
            .Setup(x => x.QueryAsync<IEnumerable<Bracelet>>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                null))
            .ReturnsAsync(expectedBracelets);

        // Act
        var result = await _braceletRepository.GetAllAsync();

        // Assert
        Assert.Equal(expectedBracelets.Count(), result.Count());
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
                new AccessPoint { Id = 1, Name = "Point A" },
                new AccessPoint { Id = 2, Name = "Point B" }
            },
            CreatedAt = _fixedDate,
            QrCode = "QR123"
        };
        var expectedId = 1;

        _mockDapper
            .Setup(x => x.QuerySingleAsync<int>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<Bracelet>(b =>
                    b.VisitorId == newBracelet.VisitorId &&
                    b.CreatedAt == newBracelet.CreatedAt)))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _braceletRepository.CreateAsync(newBracelet);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldExecuteSuccessfully()
    {
        // Arrange
        var bracelet = new Bracelet
        {
            Id = 1,
            VisitorId = 1,
            AccessPoints = new List<AccessPoint>
            {
                new AccessPoint { Id = 1, Name = "Point A" },
                new AccessPoint { Id = 2, Name = "Point B" },
                new AccessPoint { Id = 3, Name = "Point C" }
            },
            CreatedAt = _fixedDate,
            QrCode = "QR123"
        };

        _mockDapper
            .Setup(x => x.ExecuteAsync(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<Bracelet>(b => b.Id == bracelet.Id)))
            .ReturnsAsync(1);

        // Act
        await _braceletRepository.UpdateAsync(bracelet);

        // Assert
        _mockDapper.Verify(x => x.ExecuteAsync(
            _mockDbConnection.Object,
            It.IsAny<string>(),
            It.Is<Bracelet>(b => b.Id == bracelet.Id)),
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
        await _braceletRepository.DeleteAsync(1);

        // Assert
        _mockDapper.Verify(x => x.ExecuteAsync(
            _mockDbConnection.Object,
            It.IsAny<string>(),
            It.Is<object>(p => p.GetType().GetProperty("Id").GetValue(p).Equals(1))),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingBracelet_ShouldReturnNull()
    {
        // Arrange
        _mockDapper
            .Setup(x => x.QuerySingleOrDefaultAsync<Bracelet>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((Bracelet)null);

        // Act
        var result = await _braceletRepository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }
}