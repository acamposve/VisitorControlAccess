using AccessControlApi.Domain.Entities;
using AccessControlApi.Infrastructure.Wrappers;
using Moq;
using System.Data;

namespace AccessControlApi.Tests.Repositories;

public class AccessPointRepositoryTests
{
    private readonly Mock<IDbConnection> _mockDbConnection;
    private readonly Mock<IDapperWrapper> _mockDapper;
    private readonly AccessPointRepository _accessPointRepository;

    public AccessPointRepositoryTests()
    {
        _mockDbConnection = new Mock<IDbConnection>();
        _mockDapper = new Mock<IDapperWrapper>();
        _accessPointRepository = new AccessPointRepository(_mockDbConnection.Object, _mockDapper.Object);
    }

    [Fact]
    public async Task GetAllAccessPointsAsync_ShouldReturnAllAccessPoints()
    {
        // Arrange
        var expectedAccessPoints = new List<AccessPoint>
        {
            new AccessPoint
            {
                Id = 1,
                Name = "Main Gate",
                Description = "Main entrance",
                CreatedBy = 1
            },
            new AccessPoint
            {
                Id = 2,
                Name = "Side Gate",
                Description = "Side entrance",
                CreatedBy = 1
            }
        };

        _mockDapper
            .Setup(x => x.QueryAsync<IEnumerable<AccessPoint>>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                null))
            .ReturnsAsync(expectedAccessPoints);

        // Act
        var result = await _accessPointRepository.GetAllAccessPointsAsync();

        // Assert
        Assert.Equal(expectedAccessPoints, result);
    }

    [Fact]
    public async Task GetAccessPointByIdAsync_ShouldReturnAccessPoint()
    {
        // Arrange
        var expectedAccessPoint = new AccessPoint
        {
            Id = 1,
            Name = "Main Gate",
            Description = "Main entrance",
            CreatedBy = 1
        };

        _mockDapper
            .Setup(x => x.QuerySingleOrDefaultAsync<AccessPoint>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<object>(p => p.GetType().GetProperty("Id").GetValue(p).Equals(1))))
            .ReturnsAsync(expectedAccessPoint);

        // Act
        var result = await _accessPointRepository.GetAccessPointByIdAsync(1);

        // Assert
        Assert.Equal(expectedAccessPoint.Id, result.Id);
        Assert.Equal(expectedAccessPoint.Name, result.Name);
        Assert.Equal(expectedAccessPoint.Description, result.Description);
        Assert.Equal(expectedAccessPoint.CreatedBy, result.CreatedBy);
    }

    [Fact]
    public async Task CreateAccessPointAsync_ShouldReturnNewId()
    {
        // Arrange
        var newAccessPoint = new AccessPoint
        {
            Name = "New Gate",
            Description = "New entrance",
            CreatedBy = 1
        };
        var expectedId = 1;

        _mockDapper
            .Setup(x => x.QuerySingleAsync<int>(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<AccessPoint>(ap =>
                    ap.Name == newAccessPoint.Name &&
                    ap.Description == newAccessPoint.Description &&
                    ap.CreatedBy == newAccessPoint.CreatedBy)))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _accessPointRepository.CreateAccessPointAsync(newAccessPoint);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task UpdateAccessPointAsync_ShouldReturnTrue()
    {
        // Arrange
        var accessPoint = new AccessPoint
        {
            Id = 1,
            Name = "Updated Gate",
            Description = "Updated entrance",
            CreatedBy = 1
        };

        _mockDapper
            .Setup(x => x.ExecuteAsync(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<AccessPoint>(ap =>
                    ap.Id == accessPoint.Id &&
                    ap.Name == accessPoint.Name &&
                    ap.Description == accessPoint.Description &&
                    ap.CreatedBy == accessPoint.CreatedBy)))
            .ReturnsAsync(1);

        // Act
        var result = await _accessPointRepository.UpdateAccessPointAsync(accessPoint);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAccessPointAsync_NonExisting_ShouldReturnFalse()
    {
        // Arrange
        var accessPoint = new AccessPoint
        {
            Id = 999,
            Name = "Non-existing Gate",
            Description = "Non-existing entrance",
            CreatedBy = 1
        };

        _mockDapper
            .Setup(x => x.ExecuteAsync(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.IsAny<AccessPoint>()))
            .ReturnsAsync(0);

        // Act
        var result = await _accessPointRepository.UpdateAccessPointAsync(accessPoint);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAccessPointAsync_ShouldReturnTrue()
    {
        // Arrange
        _mockDapper
            .Setup(x => x.ExecuteAsync(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.Is<object>(p => p.GetType().GetProperty("Id").GetValue(p).Equals(1))))
            .ReturnsAsync(1);

        // Act
        var result = await _accessPointRepository.DeleteAccessPointAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAccessPointAsync_NonExisting_ShouldReturnFalse()
    {
        // Arrange
        _mockDapper
            .Setup(x => x.ExecuteAsync(
                _mockDbConnection.Object,
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(0);

        // Act
        var result = await _accessPointRepository.DeleteAccessPointAsync(999);

        // Assert
        Assert.False(result);
    }
}
