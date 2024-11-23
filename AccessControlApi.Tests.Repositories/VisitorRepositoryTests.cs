using AccessControlApi.Domain.Entities;
using AccessControlApi.Infrastructure.Repositories;
using AccessControlApi.Infrastructure.Wrappers;
using Moq;
using System.Data;

namespace AccessControlApi.Tests.Repositories;

public class VisitorRepositoryTests
{
    private readonly Mock<IDbConnection> _mockDbConnection;
    private readonly Mock<IDapperWrapper> _mockDapper;
    private readonly VisitorRepository _visitorRepository;

    public VisitorRepositoryTests()
    {
        _mockDbConnection = new Mock<IDbConnection>();
        _mockDapper = new Mock<IDapperWrapper>();
        _visitorRepository = new VisitorRepository(_mockDbConnection.Object, _mockDapper.Object);
    }

    [Fact]
    public async Task GetVisitorByIdAsync_ExistingId_ReturnsVisitor()
    {
        // Arrange
        var expectedVisitor = new Visitor
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            Identification = "ABC123",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockDapper
            .Setup(x => x.QuerySingleOrDefaultAsync<Visitor>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(expectedVisitor);

        // Act
        var result = await _visitorRepository.GetVisitorByIdAsync(1);

        // Assert
        Assert.Equal(expectedVisitor, result);
    }

    [Fact]
    public async Task GetVisitorByIdAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _mockDapper
            .Setup(x => x.QuerySingleOrDefaultAsync<Visitor>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((Visitor)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _visitorRepository.GetVisitorByIdAsync(999));
    }

    [Fact]
    public async Task GetAllVisitorsAsync_ReturnsAllVisitors()
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
            Identification = "ABC123",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        },
        new Visitor
        {
            Id = 2,
            Name = "Jane Doe",
            Email = "jane@example.com",
            PhoneNumber = "0987654321",
            Identification = "XYZ789",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }
    };

        _mockDapper
            .Setup(x => x.QueryAsync<IEnumerable<Visitor>>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(expectedVisitors);

        // Act
        var result = await _visitorRepository.GetAllVisitorsAsync();

        // Assert
        Assert.Equal(expectedVisitors, result);
    }

    [Fact]
    public async Task CreateVisitorAsync_ReturnsNewId()
    {
        // Arrange
        var newVisitor = new Visitor
        {
            Name = "John Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            Identification = "ABC123"
        };
        var expectedId = 1;

        _mockDapper
            .Setup(x => x.QuerySingleAsync<int>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(expectedId);

        // Act
        var result = await _visitorRepository.CreateVisitorAsync(newVisitor);

        // Assert
        Assert.Equal(expectedId, result);
    }

    [Fact]
    public async Task UpdateVisitorAsync_ExistingVisitor_ReturnsTrue()
    {
        // Arrange
        var visitor = new Visitor
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            Identification = "ABC123"
        };

        _mockDapper
            .Setup(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        // Act
        var result = await _visitorRepository.UpdateVisitorAsync(visitor);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateVisitorAsync_NonExistingVisitor_ReturnsFalse()
    {
        // Arrange
        var visitor = new Visitor
        {
            Id = 999,
            Name = "John Doe",
            Email = "john@example.com",
            PhoneNumber = "1234567890",
            Identification = "ABC123"
        };

        _mockDapper
            .Setup(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(0);

        // Act
        var result = await _visitorRepository.UpdateVisitorAsync(visitor);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteVisitorAsync_ExistingId_ReturnsTrue()
    {
        // Arrange
        _mockDapper
            .Setup(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        // Act
        var result = await _visitorRepository.DeleteVisitorAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteVisitorAsync_NonExistingId_ReturnsFalse()
    {
        // Arrange
        _mockDapper
            .Setup(x => x.ExecuteAsync(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(0);

        // Act
        var result = await _visitorRepository.DeleteVisitorAsync(999);

        // Assert
        Assert.False(result);
    }
}